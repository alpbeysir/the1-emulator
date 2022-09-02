using System;

class THE1_Emulator
{
    const string DENEME = "9 6 12 27 39 99 3 2 4 3 11 4 4 16 10 20 2 1 9 22 2 0 1 5 7 0";
    const string FIBONACCI = "1 1 4 32 11 2 43 8 32 16 10 13 0 3 31 4 30 11 8 30 5 8 31 3 32 4 30 7 9 0 0 1 32 4 93 50 4 28 0 1 18 44 9";
    const string EXPONENT = "1 4 10 5 0 2 1 12 8 1 10 17 1 3 8 38 0 3 13 9 23 3 38 4 13 13 8 38 3 1 2 1 12 8 1 10 21 0 1";
    static void Main(string[] args)
    {
        //Get program from console input
        Console.WriteLine("Enter THE1 machine inital tape status, ex. '9 6 12 27...'");
        THE1 machine = new THE1(StringToIntArray(Console.ReadLine()/*FIBONACCI*/));
        machine.Run();
        Console.ReadKey();
    }

    static int[] StringToIntArray(string input)
    {
        string[] result = input.Split(' ');
        int[] nums = new int[result.Length];

        for (int i = 0; i < nums.Length; i++)
        {
            int temp;
            bool tryResult = int.TryParse(result[i], out temp);
            if (tryResult) 
                nums[i] = temp;
            else
            {
                Console.WriteLine("Input processing error!");
                return null;
            }
        }

        return nums;
    }
}

class THE1
{
    int R1 = 0, R2 = 0, I = 0;
    int[] M;
    bool halted = false;

    //Loop detection
    int backwardJumpOffset = 0;

    public THE1(int[] _tape)
    {
        M = new int[_tape.Length];
        _tape.CopyTo(M, 0);
    }
    
    public void Run()
    {
        int cycle = 1;
        int previousI = 0;
        while (!halted && cycle < 10000)
        {  
            Console.WriteLine("----- Cycle " + cycle + " -----");
            Console.WriteLine(string.Format("R1: {0}, R2: {1}, I: {2}", R1 != -999 ? R1 : "*", R2 != -999 ? R2 :"*", I != -999 ? I : "*"));
            Console.Write("Tape: ");
            for (int n = 0; n < M.Length; n++)
                Console.Write(M[n] + " ");
            Console.Write("\n\n");

            previousI = I;
            Cycle();
            cycle++;
        }     
    }

    public void Cycle()
    {
        //Fetch
        int instruction = M[I];

        //Loop detection
        int previousI = I;

        //Decode-Execute
        switch (instruction)
        {
            case 0:
                Halt();
                break;
            case 1:
                R1 = M[I + 1]; I += 2;
                break;
            case 2:
                R2 = M[I + 1]; I += 2;
                break;
            case 3:
                R1 = M[M[I + 1]]; I += 2;
                break;
            case 4:
                R2 = M[M[I + 1]]; I += 2;
                break;
            case 5:
                R1 = R2; I += 1;
                break;
            case 6:
                R1 = M[R2]; I += 1;
                break;
            case 7:
                M[R1] = R2; I += 1;
                break;
            case 8:
                M[M[I + 1]] = R1; I += 2;
                break;
            case 9:
                I = M[I + 1];
                break;
            case 10:
                if (R1 == 0) I += 2;
                else I = M[I + 1];
                break;
            case 11:
                R1 += R2; I += 1;
                break;
            case 12:
                R1 -= R2; I += 1;
                break;
            case 13:
                R1 *= R2; I += 1;
                break;
            case 14:
                R1 /= R2; I += 1;
                break;
            case 15:
                R1 = -R1; I += 1;
                break;
            case 16:
                if (R1 == R2) R1 = 0;
                else if (R1 > R2) R1 = 1;
                else R1 = -1;
                I += 1;
                break;
            default:
                Console.WriteLine("Unrecognized instruction at " + I); Halt();
                break;
        }

        //Check if registers are within bounds, halt if otherwise
        if (R1 < -127 || R1 > 127) { R1 = -999; Halt(); }
        if (R2 < -127 || R2 > 127) { R2 = -999; Halt(); }
        if (I < 0) { I = -999; Halt(); }

        //Loop detection
        if (instruction == 9 || instruction == 10 && I < previousI)
        {
            if (backwardJumpOffset == previousI - I)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Jumped to same address twice in a row backwards, possible loop");
                Console.ResetColor();
                Console.WriteLine();
            }
            backwardJumpOffset = previousI - I;
        }
    }

    void Halt()
    {
        halted = true;
        Console.WriteLine("Halted!");
    }
}