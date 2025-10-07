using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MVCBinario.Models;

namespace MVCBinario.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //se reciben los valores a y b del formulario 
        public IActionResult Index(string a, string b)
        {

            //se validan los entradas del formulario
            List<string> errores = new List<string>();

            void Validar(string valor, string binario)
            {

                //si el valor es nulo o vacio se agrega un error
                if (string.IsNullOrEmpty(valor))
                {
                    errores.Add($"El valor de '{binario}' no puede estar vacio.");
                    return;
                }
                //si el valor tiene mas de 8 caracteres se agrega un error
                if (valor.Length > 8)
                    errores.Add($"'{binario}' no puede tener mas de 8 caracteres.");

                //si el valor tiene una longitud que no es multiplo de 2 se agrega un error
                if (valor.Length % 2 != 0)
                    errores.Add($"La longitud de '{binario}' debe ser multiplo de 2 (2, 4, 6 u 8).");

                //se usa regex para validar que el valor solo contenga 0 y 1
                if (!Regex.IsMatch(valor, "^[01]+$"))
                    errores.Add($"'{binario}' solo puede contener los caracteres 0 y 1.");
            }


            Validar(a, "a");
            Validar(b, "b");


            //se validada si hay errores y se muestran en la vista
            if (errores.Count > 0)
            {
                ViewBag.Errores = errores;
                return View();
            }
            else
            {   

                //si los valores son validos se muestra un mensaje de exito y se continua con las operaciones
                ViewBag.Exito = $"a = {a}, b = {b}";
            }

            //se iguala la longitud de los valores agregando ceros a la izquierda
            int maxLength = Math.Max(a.Length, b.Length);
            a = a.PadLeft(maxLength, '0');
            b = b.PadLeft(maxLength, '0');


            //usando funciones locales se realizan las operaciones binarias 
            string AndStrings(string x, string y)
            {

                //se recorre cada caracter de las cadenas y se aplica la operacion AND bit a bit con &&
                //si ambos caracteres son 1 se agrega un 1 al resultado, si no se agrega un 0
                string result = "";
                for (int i = 0; i < x.Length; i++)
                    result += (x[i] == '1' && y[i] == '1') ? '1' : '0';
                return result;
            }


            //operacion OR bit a bit con || 
            //si alguno de los caracteres es 1 se agrega un 1 al resultado, si no se agrega un 0
            string OrStrings(string x, string y)
            {
                string result = "";
                for (int i = 0; i < x.Length; i++)
                    result += (x[i] == '1' || y[i] == '1') ? '1' : '0';
                return result;
            }

            //operacion XOR bit a bit con !=
            //si los caracteres son diferentes se agrega un 1 al resultado, si no se agrega un 0
            string XorStrings(string x, string y)
            {
                string result = "";
                for (int i = 0; i < x.Length; i++)
                    result += (x[i] != y[i]) ? '1' : '0';
                return result;
            }

            //una vez definidas las funciones se realizan las operaciones y se convierten los resultados
            //de binario a decimal, octal y hexadecimal
            string andBin = AndStrings(a, b);
            string orBin = OrStrings(a, b);
            string xorBin = XorStrings(a, b);

            //se convierten los valores binarios a decimales para realizar las operaciones de suma y multiplicacion
            int valorA = Convert.ToInt32(a, 2);
            int valorB = Convert.ToInt32(b, 2);

            int suma = valorA + valorB;
            int multiplicacion = valorA * valorB;

            //se muestran los resultados de las operaciones en diferentes bases numericas
            string sumaBin = Convert.ToString(suma, 2);
            string sumaOct = Convert.ToString(suma, 8);
            string sumaDec = suma.ToString();
            string sumaHex = Convert.ToString(suma, 16).ToUpper();

            string multBin = Convert.ToString(multiplicacion, 2);
            string multOct = Convert.ToString(multiplicacion, 8);
            string multDec = multiplicacion.ToString();
            string multHex = Convert.ToString(multiplicacion, 16).ToUpper();


            //finalmente se pasan los resultados al view de index utilizando un ViewBag con diccionarios
            ViewBag.OperacionesBinarias = new Dictionary<string, string>
            {
                { "AND", andBin },
                { "OR", orBin },
                { "XOR", xorBin }
            };

            ViewBag.Suma = new Dictionary<string, string>
            {
                { "Binaria", sumaBin },
                { "Octal", sumaOct },
                { "Decimal", sumaDec },
                { "Hexadecimal", sumaHex }
            };

            ViewBag.Multiplicacion = new Dictionary<string, string>
            {
                { "Binaria", multBin },
                { "Octal", multOct },
                { "Decimal", multDec },
                { "Hexadecimal", multHex }
            };

            return View();
        }
    }
}
