namespace Patients.ArgProcessor;

public abstract class BaseArgumentProcessor
{
    protected TextWriter Error = Console.Error;
    protected TextReader Input = Console.In;
    protected TextWriter Output = Console.Out;


    public void Process(string[] args)
    {
        bool argsAreValid = ArgsAreValid(args);

        if (argsAreValid)
        {
            PreProcess();

            ProcessLines();

            PostProcess();
        }
    }

    protected virtual bool ArgsAreValid(string[] args)
    {
        return true;
    }

    protected virtual void PreProcess()
    {
    }

    protected virtual void ProcessLine(string line)
    {
    }


    protected virtual void PostProcess()
    {
    }


    private void ProcessLines()
    {
        string? currentLine = Input.ReadLine();

        while (currentLine != null)
        {
            ProcessLine(currentLine);

            currentLine = Input.ReadLine();
        }
    }
}