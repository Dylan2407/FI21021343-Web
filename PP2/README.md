Dylan Candia Navas FI21021343

Comandos dotnet utilizados

dotnet new sln -n PP2
dotnet new mvc -o MVCBinario
dotnet sln PP2.sln add MVCBinario/
cd MVCBinario

Paginas web:

https://learn.microsoft.com/es-mx/dotnet/api/system.convert.tostring?view=net-9.0
https://www.luisllamas.es/csharp-operadores-bitwise/
https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators

Video:

https://www.youtube.com/watch?v=tNO05yKSQcU

Prompt:

Explicame como iterar con metodos de strings caracter sobre caracter para hacer operaciones bitwise en c# mvc
ChatGPT dijo:

Perfecto, Dylan. Te voy a explicar cómo iterar sobre cadenas carácter por carácter para hacer operaciones bitwise en C#, en un contexto de MVC. Esto se hace generalmente cuando recibes números binarios como strings y quieres aplicar operaciones bitwise (AND, OR, XOR) sin convertirlos a enteros grandes.

1️⃣ Igualar longitudes de los strings

Primero, si tus dos números binarios son de diferente longitud, debes rellenar con ceros a la izquierda para que tengan la misma longitud:

string a = "1011";
string b = "110";

// Igualar longitudes
int maxLength = Math.Max(a.Length, b.Length);
a = a.PadLeft(maxLength, '0'); // a = "1011"
b = b.PadLeft(maxLength, '0'); // b = "0110"


PadLeft agrega caracteres a la izquierda hasta alcanzar la longitud deseada.

2️⃣ Iterar carácter por carácter

Para procesar cada bit, puedes recorrer la cadena usando un for y comparar los caracteres:

string AndStrings(string x, string y)
{
    string result = "";

    for (int i = 0; i < x.Length; i++)
    {
        // Si ambos son '1', resultado es '1', si no, '0'
        result += (x[i] == '1' && y[i] == '1') ? '1' : '0';
    }

    return result;
}


x[i] y y[i] acceden al carácter en la posición i del string.

La operación condicional ? : decide qué bit poner en el resultado.

result += ... va concatenando los bits en un nuevo string.

De manera similar, puedes hacer OR y XOR:

string OrStrings(string x, string y)
{
    string result = "";
    for (int i = 0; i < x.Length; i++)
        result += (x[i] == '1' || y[i] == '1') ? '1' : '0';
    return result;
}

string XorStrings(string x, string y)
{
    string result = "";
    for (int i = 0; i < x.Length; i++)
        result += (x[i] != y[i]) ? '1' : '0';
    return result;
}

3️⃣ Usando en MVC

Supongamos que tienes un HomeController y quieres mostrar el resultado en la vista:

public ActionResult Index(string a, string b)
{
    if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        return View();

    // Igualar longitudes
    int maxLength = Math.Max(a.Length, b.Length);
    a = a.PadLeft(maxLength, '0');
    b = b.PadLeft(maxLength, '0');

    ViewBag.AND = AndStrings(a, b);
    ViewBag.OR = OrStrings(a, b);
    ViewBag.XOR = XorStrings(a, b);

    return View();
}


En tu vista (Index.cshtml) simplemente haces algo como:

<p>AND: @ViewBag.AND</p>
<p>OR: @ViewBag.OR</p>
<p>XOR: @ViewBag.XOR</p>

✅ Resumen

Asegúrate de que ambos strings tengan la misma longitud usando PadLeft.

Itera con for sobre cada carácter del string.

Aplica la operación bitwise comparando '1' y '0'.

Concatenas los resultados en un nuevo string.

Puedes usar esto directamente en un controlador MVC y pasar los resultados a la vista.

Si quieres, puedo hacerte un ejemplo completo de MVC con todas las operaciones bitwise y aritméticas, ya listo para probarlo en tu proyecto.

¿Quieres que haga eso?


Preguntas:

¿Cuál es el número que resulta al multiplicar, si se introducen los valores máximos permitidos en a y b? Indíquelo en todas las bases (binaria, octal, decimal y hexadecimal).

Los valores maximos serian 11111111 y los resultados serian:

Operaciones Binarias:
AND: 11111111
OR: 11111111
XOR: 00000000
Resultado de la Suma:
Binaria: 111111110
Octal: 776
Decimal: 510
Hexadecimal: 1FE
Resultado de la Multiplicacion:
Binaria: 1111111000000001
Octal: 177001
Decimal: 65025
Hexadecimal: FE01


¿Es posible hacer las operaciones en otra capa? Si sí, ¿en cuál sería?

Si, se podria realizar dentro de los models en donde se le daria toda la logica de las operaciones bitwise y dentro del home controller solamente se realizaria el llamado lo cual permite la reutilizacion de codigo en otros controllers
