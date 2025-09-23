using System;

class Program
{

    // Metodo de suma con for
    static int SumFor(int n)
    {
        return n * (n + 1) / 2;
    }

    // Metodo de suma con iteracion
    static int SumIte(int n)
    {
        long sum = 0;
        for (int i = 1; i <= n; i++)
        {
            sum += i;
            if (sum > int.MaxValue) break;
        }
        return (int)sum;
    }

    // Metodo main
    static void Main()
    {
        Console.WriteLine("Suma con for:");


        // Suma con for ascendente y va incrementando hasta que llegue a overflow
        int forAscendenteN = 1;
        while (true)
        {
            try
            {

                //suma actual
                int sum = SumFor(forAscendenteN);
                //si la suma es negativa, hay overflow y se sale del bucle
                if (sum < 0) break;
                forAscendenteN++;
            }
            catch
            {
                break;
            }
        }
        //se muestra el resultado de la ultima suma valida
        Console.WriteLine($" Desde 1 hasta Max: {forAscendenteN - 1} Suma {SumFor(forAscendenteN - 1)}");

        // Suma con for descendente y va decrementando hasta que llegue a overflow
        int forDescendenteN = int.MaxValue;
        while (forDescendenteN > 0)
        {
            try
            {

                //suma actual
                int sum = SumFor(forDescendenteN);
                if (sum > 0)
                {   

                    //se muestra el resultado de la ultima suma valida
                    Console.WriteLine($" Desde Max hasta 1: {forDescendenteN} Suma {sum}");
                    break;
                }
                forDescendenteN--;
            }
            catch
            {
                forDescendenteN--;
            }
        }

        Console.WriteLine("\nSuma con iteracion:");

        // Suma con iteracion ascendente y va incrementando n hasta que llegue a overflow
        int iteAscendenteN = 1;
        long iteAscendenteSum = 0;
        while (true)
        {

            //suma actual y se busca que no haya overflow
            iteAscendenteSum += iteAscendenteN;
            if (iteAscendenteSum > int.MaxValue) break;
            iteAscendenteN++;
        }
        Console.WriteLine($" Desde 1 hasta Max: {iteAscendenteN - 1} Suma {SumIte(iteAscendenteN - 1)}");

        // Suma con iteracion descendente y va decrementando n hasta que llegue a overflow
        int iteDescendenteN = int.MaxValue;
        long iteDescendenteSum = 0;

        //se hace la suma descendente
        for (int i = iteDescendenteN; i >= 1; i--)
        {

            //se hace la suma y se busca que no haya overflow
            iteDescendenteSum += i;
            //si la suma es mayor que max, hay overflow y se sale del bucle
            if (iteDescendenteSum > int.MaxValue) break;
        }
        Console.WriteLine($" Desde Max hasta 1: {iteDescendenteN} Suma {(int)Math.Min(iteDescendenteSum, int.MaxValue)}");
    }
}
