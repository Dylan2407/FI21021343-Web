Dylan Candia Navas FI21021343

Comandos dotnet utilizados

dotnet new sln -n PP1

dotnet new console -n PP1consola

dotnet sln PP1.sln add PP1consola

cd PP1consola

Paginas web

https://www.alphacodingskills.com/cs/pages/cs-program-calculate-sum-of-natural-numbers.php

Prompt

Explicame como mostrar esto de manera ascendente y descendente para ambas estrategias ascendente o descedente es decir hacer esto SumFor: From 1 to Max → n: 46340 → sum: 1073720970 y SumIte: From Max to 1 → n: 2147483646 → sum: 1073741825 utilizando Int32.MaxValue

con este codigo

using System; class Program { static int SumFor(int n) { return n \* (n + 1) / 2; } static int SumIte(int n) { long sum = 0; for (int i = 1; i <= n; i++) { sum += i; if (sum > int.MaxValue) break; } return (int)sum; } }

Para mostrar los resultados de las funciones SumFor y SumIte en orden ascendente (de 1 a Max) y descendente (de Max a 1), necesitas implementar dos estrategias de recorrido y controlar los límites para evitar desbordamientos. Aquí te explico cómo hacerlo paso a paso y te doy el código completo para que lo veas en acción.

🧠 Estrategias a implementar

1. SumFor (fórmula matemática)

Ascendente: probar valores desde 1 hasta que SumFor(n) exceda int.MaxValue.

Descendente: probar valores desde int.MaxValue hacia abajo hasta que SumFor(n) sea válido.

1. SumIte (iterativa)

Ascendente: acumular desde 1 hasta que el sum exceda int.MaxValue.

Descendente: acumular desde int.MaxValue hacia abajo hasta que el sum sea válido.

static void Main()

{

Console.WriteLine("• SumFor:");

// Ascendente

int nForAsc = 1;

while (true)

{

try

{

int sum = SumFor(nForAsc);

if (sum < 0) break; // overflow

nForAsc++;

}

catch

{

break;

}

}

Console.WriteLine($"  ◦ From 1 to Max → n: {nForAsc - 1} → sum: {SumFor(nForAsc - 1)}");

// Descendente

int nForDesc = int.MaxValue;

while (nForDesc > 0)

{

try

{

int sum = SumFor(nForDesc);

if (sum > 0)

{

Console.WriteLine($"  ◦ From Max to 1 → n: {nForDesc} → sum: {sum}");

break;

}

nForDesc--;

}

catch

{

nForDesc--;

}

}

Console.WriteLine("\n• SumIte:");

// Ascendente

int nIteAsc = 1;

long sumIteAsc = 0;

while (true)

{

sumIteAsc += nIteAsc;

if (sumIteAsc > int.MaxValue) break;

nIteAsc++;

}

Console.WriteLine($"  ◦ From 1 to Max → n: {nIteAsc - 1} → sum: {SumIte(nIteAsc - 1)}");

// Descendente

int nIteDesc = int.MaxValue;

long sumIteDesc = 0;

for (int i = nIteDesc; i >= 1; i--)

{

sumIteDesc += i;

if (sumIteDesc > int.MaxValue) break;

}

Console.WriteLine($"  ◦ From Max to 1 → n: {nIteDesc} → sum: {(int)Math.Min(sumIteDesc, int.MaxValue)}");

}

}

¿Qué hace este código?

Calcula el mayor n posible para cada estrategia sin que el resultado se desborde.

Usa try-catch y validaciones para evitar errores por overflow.

Muestra los valores de n y sum para cada dirección (ascendente y descendente).

🧩 TL;DR: Lo esencial

SumFor usa una fórmula directa, pero puede desbordarse rápidamente.

SumIte es más segura pero más lenta, ya que acumula paso a paso.

Para ambas, se puede recorrer desde 1 a Max y desde Max a 1, controlando el límite de int.MaxValue.


Preguntas

¿Por qué todos los valores resultantes tanto de n como de sum difieren entre métodos (fórmula e implementación iterativa) y estrategias (ascendente y descendente)?

Al realizar distintas pruebas en ascendente se aumenta n desde 1 hasta llegar al limite y con el descendente se empieza con el Max es decir se decrementa hasta encontrar el primer n que la suma sea positva es decir debido al comportamiento de estos el overflow de cada uno no coinciden por el orden en el que se ejecutga la suma.

¿Qué cree que sucedería si se utilizan las mismas estrategias (ascendente y descendente) pero con el método recursivo de suma (SumRec)? [si desea puede implementarlo y observar qué sucede en ambos escenarios]

No llegue a probarlo bien pero considero que al intentar calcular la suma de Max seria demasiado grande para el programa ya que son millones de llamadas recursivas entonces el SumRec serviria mas para valores mas pequeños.
