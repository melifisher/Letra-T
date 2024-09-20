using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Letra_T
{
    class MainForm: Form
    {
        private GLControl glControl;
        private Button btnRotate;
        private Button btnTranslate;
        private Button btnScale;
        private TextBox txtX;
        private TextBox txtY;
        private TextBox txtZ;
        private Label lblX;
        private Label lblY;
        private Label lblZ;
        private ComboBox comboBoxObjects;
        private Panel controlPanel;

        private Escenario escenario;
        private Transformable selectedObject;

        private Matrix4 projectionMatrix;
        private Matrix4 modelViewMatrix;
        private enum TransformationType
        {
            None,
            Rotate,
            Translate,
            Scale
        }

        public MainForm()
        {
            InitializeComponent();
            //InitializeOpenGL();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "Manipular T";

            controlPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 250
            };

            glControl = new GLControl
            {
                Dock = DockStyle.Fill
            };
            glControl.Paint += GlControl_Paint;
            glControl.MouseClick += GlControl_MouseClick;

            btnRotate = new Button { Text = "Rotate", Location = new Point(10, 10), Width = 70 };
            btnTranslate = new Button { Text = "Translate", Location = new Point(90, 10), Width = 70 };
            btnScale = new Button { Text = "Scale", Location = new Point(170, 10), Width = 70 };

            txtX = new TextBox { Location = new Point(70, 40), Width = 60 };
            txtY = new TextBox { Location = new Point(70, 70), Width = 60 };
            txtZ = new TextBox { Location = new Point(70, 100), Width = 60 };

            lblX = new Label { Text = "X:", Location = new Point(10, 43) };
            lblY = new Label { Text = "Y:", Location = new Point(10, 73) };
            lblZ = new Label { Text = "Z:", Location = new Point(10, 103) };

            comboBoxObjects = new ComboBox { Location = new Point(10, 130), Width = 230 };

            btnRotate.Click += (s, e) => ApplyTransformation(TransformationType.Rotate);
            btnTranslate.Click += (s, e) => ApplyTransformation(TransformationType.Translate);
            btnScale.Click += (s, e) => ApplyTransformation(TransformationType.Scale);

            comboBoxObjects.SelectedIndexChanged += ComboBoxObjects_SelectedIndexChanged;

            controlPanel.Controls.AddRange(new Control[] {
                btnRotate, btnTranslate, btnScale,
                txtX, txtY, txtZ,
                lblX, lblY, lblZ,
                comboBoxObjects
            });

            this.Controls.Add(glControl);
            this.Controls.Add(controlPanel);

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeOpenGL();
        }
        private void InitializeOpenGL()
        {
            if (glControl == null || !glControl.IsHandleCreated)
            {
                // If GLControl is not ready, postpone initialization
                this.Load += (s, e) => InitializeOpenGL();
                return;
            }

            glControl.MakeCurrent();
            GL.ClearColor(Color.SkyBlue);
            GL.Enable(EnableCap.DepthTest);

            escenario = new Escenario();
            CrearObjetos();
            PopulateComboBox();

            float aspect = glControl.Width / (float)glControl.Height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, aspect, 1, 64);
            modelViewMatrix = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);


            glControl.Invalidate();
        }
        private void PopulateComboBox()
        {
            comboBoxObjects.Items.Clear();
            foreach (var obj in escenario.Objetos)
            {
                comboBoxObjects.Items.Add(obj.Key);
                foreach (var parte in obj.Value.Partes)
                {
                    comboBoxObjects.Items.Add($"{obj.Key} - {parte.Key}");
                }
            }
        }
        private void CrearObjetos()
        {
            //var letraT = CrearLetraT();
            //ObjetoSerializer.Guardar<Objeto>(letraT, "letraT2.json");
            escenario.AddObjeto("letraT1", ObjetoSerializer.Cargar<Objeto>("letraT.json"));
            escenario.AddObjeto("letraT2", ObjetoSerializer.Cargar<Objeto>("letraT2.json"));
        }

        private void ApplyTransformation(TransformationType type)
        {
            if (selectedObject == null)
            {
                MessageBox.Show("No object selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!float.TryParse(txtX.Text, out float x) ||
                !float.TryParse(txtY.Text, out float y) ||
                !float.TryParse(txtZ.Text, out float z))
            {
                MessageBox.Show("Invalid input. Please enter valid numbers for X, Y, and Z.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Vector3 transformVector = new Vector3(x, y, z);

            switch (type)
            {
                case TransformationType.Rotate:
                    selectedObject.Rotar(transformVector*(float)(Math.PI/180));
                    break;
                case TransformationType.Translate:
                    selectedObject.Trasladar(transformVector);
                    break;
                case TransformationType.Scale:
                    selectedObject.Escalar(new Vector3(1 + x, 1 + y, 1 + z));
                    break;
            }

            glControl.Invalidate(); // Request a redraw
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            if (!glControl.IsHandleCreated) return;

            glControl.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelViewMatrix);

            escenario.Draw();

            glControl.SwapBuffers();
        }

        private void GlControl_MouseClick(object sender, MouseEventArgs e)
        {
            Vector3 nearPoint = UnprojectPoint(e.X, e.Y, 0f);
            Vector3 farPoint = UnprojectPoint(e.X, e.Y, 1f);

            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            KeyValuePair<string, Transformable> clickedObject = PerformRayPicking(nearPoint, direction);

            if (clickedObject.Value != null)
            {
                // Actualizar la selección en el ComboBox
                if (clickedObject.Value is Objeto)
                {
                    comboBoxObjects.SelectedItem = clickedObject.Key;
                }
                else if (clickedObject.Value is Parte)
                {
                    Parte parte = (Parte)clickedObject.Value;
                    var parentObjeto = escenario.Objetos.FirstOrDefault(kv => kv.Value.Partes.ContainsValue(parte));
                    //Objeto parentObjeto = escenario.Objetos.Find(o => o.Partes.Contains(parte));
                    comboBoxObjects.SelectedItem = $"{parentObjeto.Key} - {clickedObject.Key}";
                }
                //Console.WriteLine($"Clicked object: {clickedObject.Key}"); // Debug output
            }
        }

        private Vector3 UnprojectPoint(int x, int y, float z)
        {
            Vector4 vec;

            vec.X = 2.0f * x / glControl.Width - 1;
            vec.Y = -(2.0f * y / glControl.Height - 1);
            vec.Z = z;
            vec.W = 1.0f;

            Matrix4 viewProjectionInverse = Matrix4.Invert(modelViewMatrix * projectionMatrix);

            vec = Vector4.Transform(vec, viewProjectionInverse);
            if (vec.W > 0.000001f || vec.W < -0.000001f)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec.Xyz;
        }

        private KeyValuePair<string, Transformable> PerformRayPicking(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float closestDistance = float.MaxValue;

            KeyValuePair<string, Transformable> closestObject = new KeyValuePair<string, Transformable>();
            

            foreach (var objeto in escenario.Objetos)
            {
                float distance;
                if (IntersectsObject(rayOrigin, rayDirection, objeto.Value, out distance))
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = new KeyValuePair<string, Transformable>(objeto.Key, objeto.Value);
                    }
                }

                foreach (var parte in objeto.Value.Partes)
                {
                    if (IntersectsObject(rayOrigin, rayDirection, parte.Value, out distance))
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestObject = new KeyValuePair<string, Transformable>(parte.Key,parte.Value);
                        }
                    }
                }
            }
            return closestObject;
        }

        private bool IntersectsObject(Vector3 rayOrigin, Vector3 rayDirection, Transformable obj, out float distance)
        {
            float radius = 0.5f;
            Vector3 center = obj.CenterOfMass.ToVector3() + obj.getPosition();

            Vector3 m = rayOrigin - center;
            float b = Vector3.Dot(m, rayDirection);
            float c = Vector3.Dot(m, m) - radius * radius;

            if (c > 0f && b > 0f)
            {
                distance = float.MaxValue;
                return false;
            }

            float discr = b * b - c;

            if (discr < 0f)
            {
                distance = float.MaxValue;
                return false;
            }

            distance = -b - (float)Math.Sqrt(discr);

            if (distance < 0f)
                distance = -b + (float)Math.Sqrt(discr);

            return distance >= 0f;
        }

        private void ComboBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBoxObjects.SelectedItem.ToString();
            if (string.IsNullOrEmpty(selectedItem))
            {
                //Console.WriteLine("No item selected in combo box");
                return;
            }

            Console.WriteLine($"Selected item: {selectedItem}"); // Debug output

            if (selectedItem.Contains(" - "))
            {
                // Es una parte
                string[] parts = selectedItem.Split(new string[] { " - " }, StringSplitOptions.None);
                if (escenario.Objetos.TryGetValue(parts[0], out Objeto obj) &&
                    obj.Partes.TryGetValue(parts[1], out Parte part))
                {
                    selectedObject = part;
                    Console.WriteLine($"Selected part: {parts[1]} of object {parts[0]}"); // Debug output
                }
            }
            else
            {
                // Es un objeto completo
                if (escenario.Objetos.TryGetValue(selectedItem, out Objeto obj))
                {
                    selectedObject = obj;
                    Console.WriteLine($"Selected object: {selectedItem}"); // Debug output
                }
            }
            ResetTransformInputs();
        }
        private void ResetTransformInputs()
        {
            txtX.Text = "0";
            txtY.Text = "0";
            txtZ.Text = "0";
        }
        private Objeto CrearLetraT()
        {
            var letraT = new Objeto();

            // Parte vertical de la T
            var parteVertical = new Parte();

            // Cara frontal (roja)
            parteVertical.AddPoligono("rojo", new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f,  0.1f),
            new Punto( 0.1f, -0.5f,  0.1f),
            new Punto( 0.1f,  0.0f,  0.1f),
            new Punto(-0.1f,  0.0f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Red
            ));

            // Cara trasera (verde)
            parteVertical.AddPoligono("verde", new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f, -0.1f),
            new Punto( 0.1f, -0.5f, -0.1f),
            new Punto( 0.1f,  0.0f, -0.1f),
            new Punto(-0.1f,  0.0f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Green
            ));

            // Cara izquierda (azul)
            parteVertical.AddPoligono("azul", new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f, -0.1f),
            new Punto(-0.1f, -0.5f,  0.1f),
            new Punto(-0.1f,  0.0f,  0.1f),
            new Punto(-0.1f,  0.0f, -0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Blue
            ));

            // Cara derecha (amarilla)
            parteVertical.AddPoligono("amarillo", new Poligono(
                new List<Punto> {
            new Punto(0.1f, -0.5f, -0.1f),
            new Punto(0.1f, -0.5f,  0.1f),
            new Punto(0.1f,  0.0f,  0.1f),
            new Punto(0.1f,  0.0f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Yellow
            ));

            // Cara inferior (cian)
            parteVertical.AddPoligono("cian", new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f, -0.1f),
            new Punto( 0.1f, -0.5f, -0.1f),
            new Punto( 0.1f, -0.5f,  0.1f),
            new Punto(-0.1f, -0.5f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Cyan
            ));

            // Parte horizontal de la T
            var parteHorizontal = new Parte();

            // Cara frontal (magenta)
            parteHorizontal.AddPoligono("magenta", new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f,  0.1f),
            new Punto( 0.5f,  0.0f,  0.1f),
            new Punto( 0.5f,  0.1f,  0.1f),
            new Punto(-0.5f,  0.1f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Magenta
            ));

            // Cara trasera (naranja)
            parteHorizontal.AddPoligono("naranja", new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.1f, -0.1f),
            new Punto(-0.5f,  0.1f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Orange
            ));

            // Cara superior (blanca)
            parteHorizontal.AddPoligono("blanca", new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.1f, -0.1f),
            new Punto( 0.5f,  0.1f, -0.1f),
            new Punto( 0.5f,  0.1f,  0.1f),
            new Punto(-0.5f,  0.1f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.White
            ));

            // Cara inferior (gris)
            parteHorizontal.AddPoligono("gris", new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.0f,  0.1f),
            new Punto(-0.5f,  0.0f,  0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Gray
            ));

            // Cara izquierda (púrpura)
            parteHorizontal.AddPoligono("púrpura", new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f, -0.1f),
            new Punto(-0.5f,  0.0f,  0.1f),
            new Punto(-0.5f,  0.1f,  0.1f),
            new Punto(-0.5f,  0.1f, -0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Purple
            ));

            // Cara derecha (lima)
            parteHorizontal.AddPoligono("lima", new Poligono(
                new List<Punto> {
            new Punto(0.5f,  0.0f, -0.1f),
            new Punto(0.5f,  0.0f,  0.1f),
            new Punto(0.5f,  0.1f,  0.1f),
            new Punto(0.5f,  0.1f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Lime
            ));

            letraT.AddParte("vertical", parteVertical);
            letraT.AddParte("horizontal", parteHorizontal);

            return letraT;
        }

    }
}
