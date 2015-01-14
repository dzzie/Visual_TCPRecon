namespace Visual_TCPRecon.Interfaces
{
    public interface IScriptableComponent
    {
        Form1 Parent{ get; set; }            
        void DoSomething(string parameter);
    }
}
