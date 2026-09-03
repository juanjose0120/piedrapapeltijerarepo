
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PIEDRA
{
    public partial class Form1 : Form
    {
        // Matriz de transición de Markov
        int[,] matriz = new int[3, 3];


        int ultimaEleccion = -1;

        Random random = new Random();

        // Vector que guarda cada jugada
        List<(int jugador, int computadora)> historial = new List<(int, int)>();

        // Contadores de elecciones
        int[] conteoJugador = new int[3];      
        int[] conteoComputadora = new int[3];
        // Contadores de resultados del jugador
        int victorias = 0;
        int derrotas = 0;
        int empates = 0;

        string rutaArchivo = "C:\\Users\\jujov\\OneDrive\\doc_lenguajes\\piedra, papel\\p,p,t.txt";
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


            if (ultimaEleccion != -1)
            {
                matriz[ultimaEleccion, eleccionJugador]++;
            }

            // La computadora elige usando Markov
            int eleccionComputadora = elegirMarkov();


            ELECCION.Text += "\r\nLa computadora eligió "
                + nombreEleccion(eleccionComputadora);


            if (eleccionJugador == eleccionComputadora)
            {
                ELECCION.Text += "\r\nEmpate";
                empates++;
            }
            else if (
                (eleccionJugador == 0 && eleccionComputadora == 2) ||
                (eleccionJugador == 1 && eleccionComputadora == 0) ||
                (eleccionJugador == 2 && eleccionComputadora == 1)
            )
            {
                ELECCION.Text += "\r\n¡Ganaste!";
                victorias++;
            }
            else
            {
                ELECCION.Text += "\r\nGanó la computadora";
                derrotas++;
            }
            ultimaEleccion = eleccionJugador;

            // Guardar la jugada en el historial 
            historial.Add((eleccionJugador, eleccionComputadora));
            conteoJugador[eleccionJugador]++;
            conteoComputadora[eleccionComputadora]++;
            guardarResultados();
        }

        private void guardarResultados()
        {
            using (StreamWriter writer = new StreamWriter(rutaArchivo, false))
            {
                writer.WriteLine("=== HISTORIAL DE JUGADAS ===");
                for (int i = 0; i < historial.Count; i++)
                {
                    writer.WriteLine(
                        $"Ronda {i + 1}: Jugador eligió {nombreEleccion(historial[i].jugador)} "
                        + $"- Computadora eligió {nombreEleccion(historial[i].computadora)}"
                    );
                }


                writer.WriteLine();
                writer.WriteLine("=== CONTEO DE ELECCIONES ===");
                writer.WriteLine("Jugador:");
                writer.WriteLine($"  Piedra: {conteoJugador[0]}");
                writer.WriteLine($"  Papel: {conteoJugador[1]}");
                writer.WriteLine($"  Tijera: {conteoJugador[2]}");

                writer.WriteLine("Computadora:");
                writer.WriteLine($"  Piedra: {conteoComputadora[0]}");
                writer.WriteLine($"  Papel: {conteoComputadora[1]}");
                writer.WriteLine($"  Tijera: {conteoComputadora[2]}");

                writer.WriteLine();
                writer.WriteLine("=== RESULTADOS DEL JUGADOR ===");
                writer.WriteLine($"  Victorias: {victorias}");
                writer.WriteLine($"  Derrotas: {derrotas}");
                writer.WriteLine($"  Empates: {empates}");
            }
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
            // Si no tenemos una jugada anterior
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

            // Elegimos la jugada que vence la predicción
            if (prediccion == 0)
            {
                return 1; 
            }
            else if (prediccion == 1)
            {
                return 2; 
            }
            else
            {
                return 0; 
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
    }
}


