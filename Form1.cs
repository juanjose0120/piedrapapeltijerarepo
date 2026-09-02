
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PIEDRA
{
    public partial class Form1 : Form
    {
        // 0 = Piedra
        // 1 = Papel
        // 2 = Tijera

        // Matriz de transición de Markov
        int[,] matriz = new int[3, 3];

        // Última elección del jugador
        int ultimaEleccion = -1;

        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void jugar(int eleccionJugador)
        {
            // Guardar la elección del jugador
            ELECCION.Text = "Tu elección fue " + nombreEleccion(eleccionJugador);

            // Si ya existe una elección anterior,
            // actualizamos la matriz de transición
            if (ultimaEleccion != -1)
            {
                matriz[ultimaEleccion, eleccionJugador]++;
            }

            // La computadora elige usando Markov
            int eleccionComputadora = elegirMarkov();

            // Mostrar elección de la computadora
            ELECCION.Text += "\r\nLa computadora eligió "
                + nombreEleccion(eleccionComputadora);

            // Determinar ganador
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
            }
            else
            {
                ELECCION.Text += "\r\nGanó la computadora";
            }

            // Guardar la elección actual como última elección
            ultimaEleccion = eleccionJugador;
        }

        private string nombreEleccion(int eleccion)
        {
            if (eleccion == 0)
                return "Piedra";

            if (eleccion == 1)
                return "Papel";

            return "Tijera";
        }

        private int elegirMarkov()
        {
            // Si no tenemos una jugada anterior,
            // elegimos al azar
            if (ultimaEleccion == -1)
            {
                return random.Next(0, 3);
            }

            int piedra = matriz[ultimaEleccion, 0];
            int papel = matriz[ultimaEleccion, 1];
            int tijera = matriz[ultimaEleccion, 2];

            int total = piedra + papel + tijera;

            // Si no hay información suficiente
            if (total == 0)
            {
                return random.Next(0, 3);
            }

            // Elegimos cuál es la jugada más probable del jugador
            int prediccion;

            if (piedra >= papel && piedra >= tijera)
            {
                prediccion = 0; // Piedra
            }
            else if (papel >= piedra && papel >= tijera)
            {
                prediccion = 1; // Papel
            }
            else
            {
                prediccion = 2; // Tijera
            }

            // Elegimos la jugada que vence la predicción
            if (prediccion == 0)
            {
                return 1; // Si predice Piedra, computadora usa Papel
            }
            else if (prediccion == 1)
            {
                return 2; // Si predice Papel, computadora usa Tijera
            }
            else
            {
                return 0; // Si predice Tijera, computadora usa Piedra
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Piedra
            jugar(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Papel
            jugar(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Tijera
            jugar(2);
        }
    }
}


