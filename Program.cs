using System;
using System.Threading;
using static System.Collections.Specialized.BitVector32;

namespace Singleton
{
    // Esta implementación de Singleton se denomina "bloqueo de doble comprobación". Es seguro
    // en un entorno multiproceso y proporciona una inicialización diferida para el Objeto Singelton

    class Singleton
    {
        private Singleton() { }

        private static Singleton _instancesss;

        // Ahora tenemos un objeto de bloqueo que se usará para sincronizar subprocesos
        // durante el primer acceso al Singleton.
        private static readonly object _lock = new object();

        public static Singleton GetInstance(string value)
        {
            // Este condicional es necesaria para evitar que los subprocesos tropiecen con
            // el bloqueador una vez que la instancia esté lista
            if (_instancesss == null)
            {
                // Ahora, imagina que el programa acaba de ser lanzado. 
                // Dado que todavía no hay ninguna instancia de Singleton, varios subprocesos pueden
                // Pasar simultáneamente el condicional anterior y llegar a este
                // punto casi al mismo tiempo. El primero de ellos adquirirá el lock y 
                // seguirá adelante, mientras que el resto esperará aquí.
                lock (_lock)
                {
                    //El primer hilo en adquirir el bloqueo, llega a este
                    //condicional, entra y crea la insatancia Singleton.
                    //Una vez que sale del bloque de lock, un subproceso que
                    //podría haber estado esperando de que el lock se libere entonces puede
                    //entrar en esta sección.Pero dado que el campo  Singleton ya esta inicializado
                    //, el subproceso no creará un nuevo
                    //objeto.
                    if (_instancesss == null)
                    {
                        _instancesss = new Singleton();
                        _instancesss.Value = value;
                    }
                }
            }
            return _instancesss;
        }

        // We'll use this property to prove that our Singleton really works.
        public string Value { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // The client code.

            Console.WriteLine(
                "{0}\n{1}\n\n{2}\n",
                "If you see the same value, then singleton was reused (yay!)",
                "If you see different values, then 2 singletons were created (booo!!)",
                "RESULT:"
            );

            Thread process1 = new Thread(() =>
            {
                TestSingleton("FOO");
            });
            Thread process2 = new Thread(() =>
            {
                TestSingleton("BAR");
            });

            process1.Start();
            process2.Start();

            process1.Join();
            process2.Join();
        }

        public static void TestSingleton(string value)
        {
            Singleton singleton = Singleton.GetInstance(value);
            Console.WriteLine(singleton.Value);
        }
    }
}
/* 
 El patrón de diseño Singleton se utiliza en situaciones en las que se
necesita garantizar que una clase tenga una única instancia y proporcionar
un punto de acceso global a esa instancia.
Al momento de desarrolar una app que necesita cargar configuraciones globales, 
como la configuración del servidor, credenciales de autenticación,
parámetros de conexión a una base de datos, o ajustes de nivel de aplicación que son compartidos por varios componentes.
Utilizar el patrón Singleton sería una forma efectiva de garantizar que todas las partes de la aplicación accedan
a la misma instancia de configuración sin necesidad de cargarla varias veces desde el disco o desde fuentes externas.
 

 */