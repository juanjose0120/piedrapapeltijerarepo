using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PIEDRA
{
    public partial class Form1 : Form
    {
        // 0 = Piedra, 1 = Papel, 2 = Tijera
        int[,] matriz = new int[3, 3];
        int ultimaEleccion = -1;
        int jugadasAprendizaje = 0;
        const int LIMITE_APRENDIZAJE = 100;
        Random random = new Random();
        // Guarda las primeras 100 jugadas
        // Tamaño en el que diseñaste originalmente la ventana
        int anchoOriginal = 897;
        int altoOriginal = 761;
        List<(int jugador, int computadora)> historial =
            new List<(int, int)>();
        int[] conteoJugador = new int[3];
        int[] conteoComputadora = new int[3];
        int victorias = 0;
        int derrotas = 0;
        int empates = 0;
        // El archivo se creará en la carpeta donde se ejecuta el programa
        string rutaArchivo = Path.Combine(
            Application.StartupPath,
            "conocimiento_piedra_papel_tijera.txt"
        );
        public Form1()
        {
            InitializeComponent();
            // Hace que las imágenes no se deformen ni se corten
            pictureUsuario.SizeMode = PictureBoxSizeMode.Zoom;
            pictureMaquina.SizeMode = PictureBoxSizeMode.Zoom;

            // Quita el fondo gris de los PictureBox
            pictureUsuario.BackColor = System.Drawing.Color.Transparent;
            pictureMaquina.BackColor = System.Drawing.Color.Transparent;

            // Detectar cuando cambia el tamaño de la ventana
            this.Resize += Form1_Resize;
            // Recuperar lo aprendido anteriormente
            cargarConocimiento();
        }
        private void ajustarControles()
        {
            float escalaX = (float)this.ClientSize.Width / anchoOriginal;
            float escalaY = (float)this.ClientSize.Height / altoOriginal;
            // Imagen del usuario - izquierda
            pictureUsuario.Location = new System.Drawing.Point(
                (int)(41 * escalaX),
                (int)(328 * escalaY)
            );
            pictureUsuario.Size = new System.Drawing.Size(
                (int)(160 * escalaX),
                (int)(186 * escalaY)
            );
            // Imagen de la computadora - derecha
            pictureMaquina.Location = new System.Drawing.Point(
                (int)(712 * escalaX),
                (int)(326 * escalaY)
            );
            pictureMaquina.Size = new System.Drawing.Size(
                (int)(164 * escalaX),
                (int)(189 * escalaY)
            );
            // Botón Piedra
            button1.Location = new System.Drawing.Point(
                (int)(259 * escalaX),
                (int)(244 * escalaY)
            );
            button1.Size = new System.Drawing.Size(
                (int)(399 * escalaX),
                (int)(60 * escalaY)
            );
            // Botón Papel
            button2.Location = new System.Drawing.Point(
                (int)(259 * escalaX),
                (int)(344 * escalaY)
            );
            button2.Size = new System.Drawing.Size(
                (int)(399 * escalaX),
                (int)(60 * escalaY)
            );
            // Botón Tijera
            button3.Location = new System.Drawing.Point(
                (int)(259 * escalaX),
                (int)(444 * escalaY)
            );
            button3.Size = new System.Drawing.Size(
                (int)(399 * escalaX),
                (int)(60 * escalaY)
            );
            // TextBox / Label donde aparece la elección
            ELECCION.Location = new System.Drawing.Point(
                (int)(259 * escalaX),
                (int)(567 * escalaY)
            );
            ELECCION.Size = new System.Drawing.Size(
                (int)(395 * escalaX),
                (int)(129 * escalaY)
            );
            button4.Location = new System.Drawing.Point(
                (int)(189 * escalaX),
                (int)(109 * escalaY)
            );

            button4.Size = new System.Drawing.Size(
                (int)(520 * escalaX),
                (int)(70 * escalaY)
            );
            button4.Font = new System.Drawing.Font(
                button4.Font.FontFamily,
                28 * escalaY,
                button4.Font.Style
            );
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ELECCION.Text =
                "Jugadas aprendidas: " + jugadasAprendizaje +
                " de " + LIMITE_APRENDIZAJE;
            ajustarControles();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            ajustarControles();
        }
        private void jugar(int eleccionJugador)
        {
            mostrarImagen(pictureUsuario, eleccionJugador);
            ELECCION.Text =
                "Tu elección fue " + nombreEleccion(eleccionJugador);
            
             /* Solamente se modifica la matriz durante las
             * primeras 100 jugadas.*/
            if (jugadasAprendizaje < LIMITE_APRENDIZAJE)
            {
                if (ultimaEleccion != -1)
                {
                    matriz[ultimaEleccion, eleccionJugador]++;
                }
            }
            // La computadora utiliza la matriz aprendida
            int eleccionComputadora = elegirMarkov();
            mostrarImagen(pictureMaquina, eleccionComputadora);

            ELECCION.Text +=
                "\r\nLa computadora eligió " +
                nombreEleccion(eleccionComputadora);
            bool jugadorGano = false;
            bool computadoraGano = false;
            if (eleccionJugador == eleccionComputadora)
            {
                ELECCION.Text += "\r\nEmpate";
            }
            else if (
                (eleccionJugador == 0 && eleccionComputadora == 2) ||
                (eleccionJugador == 1 && eleccionComputadora == 0) ||
                (eleccionJugador == 2 && eleccionComputadora == 1)
            )
            {
                ELECCION.Text += "\r\n¡Ganaste!";
                jugadorGano = true;
            }
            else
            {
                ELECCION.Text += "\r\nGanó la computadora";
                computadoraGano = true;
            }
             /* Guardar información únicamente mientras
             * todavía se encuentra aprendiendo.*/
            if (jugadasAprendizaje < LIMITE_APRENDIZAJE)
            {
                historial.Add(
                    (eleccionJugador, eleccionComputadora)
                );

                conteoJugador[eleccionJugador]++;
                conteoComputadora[eleccionComputadora]++;

                if (jugadorGano)
                {
                    victorias++;
                }
                else if (computadoraGano)
                {
                    derrotas++;
                }
                else
                {
                    empates++;
                }

                jugadasAprendizaje++;
                 /* Se guarda la jugada actual para que, si se
                 * cierra el programa, continúe correctamente*/
                ultimaEleccion = eleccionJugador;
                guardarConocimiento();
            }
            else
            {
                 /* Después de 100 jugadas se actualiza solamente
                 * para saber cuál fue la jugada anterior.
                 * La matriz ya no cambia.*/
                ultimaEleccion = eleccionJugador;
            }
            ELECCION.Text +=
                "\r\nJugadas aprendidas: " +
                jugadasAprendizaje + " de " +
                LIMITE_APRENDIZAJE;
            if (jugadasAprendizaje >= LIMITE_APRENDIZAJE)
            {
                ELECCION.Text +=
                    "\r\nAprendizaje terminado. Ahora utiliza " +
                    "el conocimiento guardado.";
            }
        }
        private int elegirMarkov()
        {
            // Si todavía no existe una jugada anterior
            if (ultimaEleccion == -1)
            {
                return random.Next(0, 3);
            }
            int piedra = matriz[ultimaEleccion, 0];
            int papel = matriz[ultimaEleccion, 1];
            int tijera = matriz[ultimaEleccion, 2];
            int total = piedra + papel + tijera;
            // Si no hay información para esa jugada
            if (total == 0)
            {
                return random.Next(0, 3);
            }
            int prediccion;
            // Se busca qué movimiento es el más probable
            if (piedra >= papel && piedra >= tijera)
            {
                prediccion = 0;
            }
            else if (papel >= piedra && papel >= tijera)
            {
                prediccion = 1;
            }
            else
            {
                prediccion = 2;
            }

            // La computadora selecciona lo que vence la predicción
            if (prediccion == 0)
            {
                return 1; // Papel vence a Piedra
            }
            else if (prediccion == 1)
            {
                return 2; // Tijera vence a Papel
            }
            else
            {
                return 0; // Piedra vence a Tijera
            }
        }
        private void guardarConocimiento()
        {
            try
            {
                using (StreamWriter writer =
                    new StreamWriter(rutaArchivo, false))
                {
                     /* Esta parte contiene la información que
                     * el programa necesita para continuar*/
                    writer.WriteLine("DATOS_DEL_PROGRAMA");
                    writer.WriteLine(
                        "Jugadas=" + jugadasAprendizaje
                    );
                    writer.WriteLine(
                        "UltimaEleccion=" + ultimaEleccion
                    );
                    writer.WriteLine(
                        "ConteoJugador=" +
                        conteoJugador[0] + "," +
                        conteoJugador[1] + "," +
                        conteoJugador[2]
                    );
                    writer.WriteLine(
                        "ConteoComputadora=" +
                        conteoComputadora[0] + "," +
                        conteoComputadora[1] + "," +
                        conteoComputadora[2]
                    );
                    writer.WriteLine("Victorias=" + victorias);
                    writer.WriteLine("Derrotas=" + derrotas);
                    writer.WriteLine("Empates=" + empates);
                    // Guardar la matriz de Markov
                    writer.WriteLine("Matriz0=" +
                        matriz[0, 0] + "," +
                        matriz[0, 1] + "," +
                        matriz[0, 2]);
                    writer.WriteLine("Matriz1=" +
                        matriz[1, 0] + "," +
                        matriz[1, 1] + "," +
                        matriz[1, 2]);
                    writer.WriteLine("Matriz2=" +
                        matriz[2, 0] + "," +
                        matriz[2, 1] + "," +
                        matriz[2, 2]);
                    writer.WriteLine();
                    writer.WriteLine("HISTORIAL");
                    for (int i = 0; i < historial.Count; i++)
                    {
                        writer.WriteLine(
                            (i + 1) + "," +
                            historial[i].jugador + "," +
                            historial[i].computadora
                        );
                    }
                    writer.WriteLine();
                    writer.WriteLine(
                        "SIGNIFICADO: 0=Piedra, 1=Papel, 2=Tijera"
                    );
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(
                    "No se pudo guardar el archivo:\n" +
                    error.Message
                );
            }
        }
        private void cargarConocimiento()
        {
            // Si todavía no existe, comienza desde cero
            if (!File.Exists(rutaArchivo))
            {
                return;
            }
            try
            {
                string[] lineas = File.ReadAllLines(rutaArchivo);
                historial.Clear();
                bool leyendoHistorial = false;
                foreach (string lineaOriginal in lineas)
                {
                    string linea = lineaOriginal.Trim();
                    if (linea == "HISTORIAL")
                    {
                        leyendoHistorial = true;
                        continue;
                    }
                    if (linea.StartsWith("SIGNIFICADO"))
                    {
                        leyendoHistorial = false;
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(linea))
                    {
                        continue;
                    }
                    if (leyendoHistorial)
                    {
                        cargarJugadaHistorial(linea);
                    }
                    else
                    {
                        cargarDato(linea);
                    }
                }
                // Seguridad para que nunca pase de 100
                if (jugadasAprendizaje > LIMITE_APRENDIZAJE)
                {
                    jugadasAprendizaje = LIMITE_APRENDIZAJE;
                }
                MessageBox.Show(
                    "Conocimiento recuperado correctamente.\n" +
                    "Jugadas aprendidas: " +
                    jugadasAprendizaje + " de " +
                    LIMITE_APRENDIZAJE
                );
            }
            catch (Exception error)
            {
                MessageBox.Show(
                    "No se pudo cargar el conocimiento:\n" +
                    error.Message
                );
            }
        }
        private void cargarDato(string linea)
        {
            string[] partes = linea.Split('=');

            if (partes.Length != 2)
            {
                return;
            }
            string nombre = partes[0];
            string valor = partes[1];
            switch (nombre)
            {
                case "Jugadas":
                    jugadasAprendizaje = int.Parse(valor);
                    break;

                case "UltimaEleccion":
                    ultimaEleccion = int.Parse(valor);
                    break;

                case "ConteoJugador":
                    cargarVector(valor, conteoJugador);
                    break;

                case "ConteoComputadora":
                    cargarVector(valor, conteoComputadora);
                    break;

                case "Victorias":
                    victorias = int.Parse(valor);
                    break;

                case "Derrotas":
                    derrotas = int.Parse(valor);
                    break;

                case "Empates":
                    empates = int.Parse(valor);
                    break;

                case "Matriz0":
                    cargarFilaMatriz(0, valor);
                    break;

                case "Matriz1":
                    cargarFilaMatriz(1, valor);
                    break;

                case "Matriz2":
                    cargarFilaMatriz(2, valor);
                    break;
            }
        }
        private void cargarVector(string texto, int[] vector)
        {
            string[] numeros = texto.Split(',');

            if (numeros.Length == 3)
            {
                vector[0] = int.Parse(numeros[0]);
                vector[1] = int.Parse(numeros[1]);
                vector[2] = int.Parse(numeros[2]);
            }
        }
        private void cargarFilaMatriz(int fila, string texto)
        {
            string[] numeros = texto.Split(',');

            if (numeros.Length == 3)
            {
                matriz[fila, 0] = int.Parse(numeros[0]);
                matriz[fila, 1] = int.Parse(numeros[1]);
                matriz[fila, 2] = int.Parse(numeros[2]);
            }
        }
        private void cargarJugadaHistorial(string linea)
        {
            string[] datos = linea.Split(',');
            if (datos.Length != 3)
            {
                return;
            }
            int jugador = int.Parse(datos[1]);
            int computadora = int.Parse(datos[2]);

            historial.Add((jugador, computadora));
        }
        private string nombreEleccion(int eleccion)
        {
            if (eleccion == 0)
            {
                return "Piedra";
            }
            if (eleccion == 1)
            {
                return "Papel";
            }
            return "Tijera";
        }

        private void mostrarImagen(PictureBox picture, int eleccion)
        {
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.BackColor = System.Drawing.Color.Transparent;
            if (eleccion == 0)
            {
                picture.Image = Properties.Resources.Piedra_1;
            }
            else if (eleccion == 1)
            {
                picture.Image = Properties.Resources.papel_1;
            }
            else
            {
                picture.Image = Properties.Resources.Tijera_1;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            jugar(0);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            jugar(1);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            jugar(2);
        }
        private void boton_MouseDown(object sender, MouseEventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                boton.Top += 2;
                boton.Left += 2;
            }
        }
        private void boton_MouseUp(object sender, MouseEventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                boton.Top -= 2;
                boton.Left -= 2;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureMaquina_Click(object sender, EventArgs e)
        {

        }

        private void pictureUsuario_Click(object sender, EventArgs e)
        {

        }

        private void ELECCION_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
