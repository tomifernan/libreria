using System;
using System.Collections.Generic;

namespace LibreriaOnline
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Catalogo catalogo = new Catalogo();
            catalogo.AgregarLibro(new Libro("C# para Principiantes", "Tecnología", 100, 10));
            catalogo.AgregarLibro(new Libro("Aprende Python", "Tecnología", 120, 5));
            catalogo.AgregarLibro(new Libro("Historia de la Literatura", "Literatura", 80, 8));

            Usuario usuario = new Usuario("tomas", "12345");
            Console.WriteLine("Ingrese su nombre de usuario:");
            string nombreUsuario = Console.ReadLine();
            Console.WriteLine("Ingrese su contraseña:");
            string contrasena = Console.ReadLine();

            if (!usuario.IniciarSesion(nombreUsuario, contrasena))
            {
                Console.WriteLine("Credenciales incorrectas.");
                return;
            }

          
            Console.WriteLine("Seleccione cual:");
            string[] rubros = { "Tecnología", "Literatura" };
            for (int i = 0; i < rubros.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {rubros[i]}");
            }

            int seleccionRubro = int.Parse(Console.ReadLine()) - 1;
            if (seleccionRubro < 0 || seleccionRubro >= rubros.Length)
            {
                Console.WriteLine("Selección  inválida.");
                return;
            }
            string rubroSeleccionado = rubros[seleccionRubro];

           
            Console.WriteLine($"Libros que hay {rubroSeleccionado}:");
            List<Libro> librosDisponibles = catalogo.ObtenerLibrosPorRubro(rubroSeleccionado);
            if (librosDisponibles.Count == 0)
            {
                Console.WriteLine("No hay libros disponibles .");
                return;
            }

            for (int i = 0; i < librosDisponibles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {librosDisponibles[i]}");
            }

           
            List<Libro> carrito = new List<Libro>();
            while (true)
            {
                Console.WriteLine("Seleccione un libro para agregar al carrito (0 para terminar):");
                int seleccionLibro = int.Parse(Console.ReadLine()) - 1;
                if (seleccionLibro == -1) break;
                if (seleccionLibro < 0 || seleccionLibro >= librosDisponibles.Count)
                {
                    Console.WriteLine("Selección de libro inválida.");
                    continue;
                }

                Console.WriteLine("Ingrese la cantidad:");
                int cantidad = int.Parse(Console.ReadLine());

                if (cantidad <= 0)
                {
                    Console.WriteLine("La cantidad debe ser mayor a 0.");
                    continue;
                }

                Libro libroSeleccionado = librosDisponibles[seleccionLibro];
                carrito.Add(new Libro(libroSeleccionado.Titulo, libroSeleccionado.Rubro, libroSeleccionado.Precio, cantidad));
            }

           
            Console.WriteLine("¿Confirma la compra? (Si/No)");
            char confirmacion = Char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine(); 
            if (confirmacion != 'S')
            {
                Console.WriteLine("Compra cancelada.");
                return;
            }

            
            decimal totalCompra = 0;
            foreach (var item in carrito)
            {
                if (!catalogo.VerificarStock(item.Titulo, item.Cantidad))
                {
                    Console.WriteLine($"No hay suficiente stock para '{item.Titulo}'. Por favor, ajuste la cantidad o elimine el libro.");
                    return;
                }
                totalCompra += item.Precio * item.Cantidad;
            }

            
            Console.WriteLine("Ingrese número de tarjeta:");
            string numeroTarjeta = Console.ReadLine();
            Console.WriteLine("Ingrese fecha de vencimiento (00/00):");
            string fechaVencimiento = Console.ReadLine();
            Console.WriteLine("Ingrese código de seguridad:");
            string codigoSeguridad = Console.ReadLine();

          
            Pago pago = new Pago(numeroTarjeta, fechaVencimiento, codigoSeguridad);
            Console.WriteLine($"Pago realizado con éxito. Total de la compra: ${totalCompra}");

            
            foreach (var item in carrito)
            {
                catalogo.DescontarStock(item.Titulo, item.Cantidad);
            }

            Console.WriteLine("Compra completada exitosamente.");
        }
    }

    class Usuario
    {
        public string NombreUsuario { get; }
        private string Contrasena { get; }

        public Usuario(string nombreUsuario, string contrasena)
        {
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
        }

        public bool IniciarSesion(string nombreUsuario, string contrasena)
        {
            return NombreUsuario == nombreUsuario && Contrasena == contrasena;
        }
    }

    class Libro
    {
        public string Titulo { get; }
        public string Rubro { get; }
        public decimal Precio { get; }
        public int Cantidad { get; set; }

        public Libro(string titulo, string rubro, decimal precio, int cantidad = 0)
        {
            Titulo = titulo;
            Rubro = rubro;
            Precio = precio;
            Cantidad = cantidad;
        }

        public override string ToString()
        {
            return $"{Titulo} - {Precio:C} ({Cantidad} en stock)";
        }
    }

    class Catalogo
    {
        private List<Libro> libros = new List<Libro>();

        public void AgregarLibro(Libro libro)
        {
            libros.Add(libro);
        }

        public List<Libro> ObtenerLibrosPorRubro(string rubro)
        {
            return libros.FindAll(libro => libro.Rubro == rubro);
        }

        public bool VerificarStock(string titulo, int cantidad)
        {
            Libro libro = libros.Find(libro => libro.Titulo == titulo);
            return libro != null && libro.Cantidad >= cantidad;
        }

        public void DescontarStock(string titulo, int cantidad)
        {
            Libro libro = libros.Find(libro => libro.Titulo == titulo);
            if (libro != null)
            {
                libro.Cantidad -= cantidad;
            }
        }
    }

    class Pago
    {
        public string NumeroTarjeta { get; }
        public string FechaVencimiento { get; }
        public string CodigoSeguridad { get; }

        public Pago(string numeroTarjeta, string fechaVencimiento, string codigoSeguridad)
        {
            NumeroTarjeta = numeroTarjeta;
            FechaVencimiento = fechaVencimiento;
            CodigoSeguridad = codigoSeguridad;
        }
    }
}
