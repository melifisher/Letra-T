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
            //glControl.MouseClick += GlControl_MouseClick;

            btnRotate = new Button { Text = "Rotar", Location = new Point(10, 100), Width = 70 };
            btnTranslate = new Button { Text = "Trasladar", Location = new Point(90, 100), Width = 70 };
            btnScale = new Button { Text = "Escalar", Location = new Point(170, 100), Width = 70 };

            txtX = new TextBox { Location = new Point(70, 10), Width = 60 };
            txtY = new TextBox { Location = new Point(70, 40), Width = 60 };
            txtZ = new TextBox { Location = new Point(70, 70), Width = 60 };

            lblX = new Label { Text = "X:", Location = new Point(10, 13) };
            lblY = new Label { Text = "Y:", Location = new Point(10, 43) };
            lblZ = new Label { Text = "Z:", Location = new Point(10, 73) };

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
            modelViewMatrix = Matrix4.LookAt(0, 0, 3, 0, 0, 0, 0, 1, 0);

            glControl.Invalidate();
        }
        private void PopulateComboBox()
        {
            comboBoxObjects.Items.Clear();
            comboBoxObjects.Items.Add("Escenario");
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
            Objeto letraT1 = ObjetoSerializer.Cargar<Objeto>("letraT.json");
            letraT1.Trasladar(new Vector3(0.7f, 0, 0));
            escenario.AddObjeto("letraT1", letraT1);
            Objeto letraT2 = ObjetoSerializer.Cargar<Objeto>("letraT2.json");
            letraT2.Trasladar(new Vector3(-0.7f, 0, 0));
            escenario.AddObjeto("letraT2", letraT2);
        }

        private void ApplyTransformation(TransformationType type)
        {

            if (!float.TryParse(txtX.Text, out float x) ||
                !float.TryParse(txtY.Text, out float y) ||
                !float.TryParse(txtZ.Text, out float z))
            {
                MessageBox.Show("Invalid input. Please enter valid numbers for X, Y, and Z.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Vector3 transformVector = new Vector3(x, y, z);


            if (selectedObject == null)
            {
                switch (type)
                {
                    case TransformationType.Rotate:
                        escenario.Rotar(transformVector * (float)(Math.PI / 180));
                        break;
                    case TransformationType.Translate:
                        escenario.Trasladar(transformVector);
                        break;
                    case TransformationType.Scale:
                        escenario.Escalar(transformVector);
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case TransformationType.Rotate:
                        selectedObject.Rotar(transformVector * (float)(Math.PI / 180));
                        break;
                    case TransformationType.Translate:
                        selectedObject.Trasladar(transformVector);
                        break;
                    case TransformationType.Scale:
                        selectedObject.Escalar(transformVector);
                        break;
                }

            }

            glControl.Invalidate();
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

        private void ComboBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBoxObjects.SelectedItem.ToString();
            if (string.IsNullOrEmpty(selectedItem))
            {
                Console.WriteLine("No item selected in combo box");
                return;
            }

            Console.WriteLine($"Selected item: {selectedItem}"); // Debug output

            if (selectedItem.Contains(" - "))
            {
                string[] parts = selectedItem.Split(new string[] { " - " }, StringSplitOptions.None);
                if (escenario.Objetos.TryGetValue(parts[0], out Objeto obj) &&
                    obj.Partes.TryGetValue(parts[1], out Parte part))
                {
                    selectedObject = part;
                    Console.WriteLine($"Selected part: {parts[1]} of object {parts[0]}");
                }
            }
            else
            {
                if (escenario.Objetos.TryGetValue(selectedItem, out Objeto obj))
                {
                    selectedObject = obj;
                    Console.WriteLine($"Selected object: {selectedItem}");
                }
                else
                {
                    selectedObject = escenario;
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
