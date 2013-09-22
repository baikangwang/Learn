namespace WCFSample.Contract
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "WCFSample.Services")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double d1, double d2);
        [OperationContract]
        double Subtract(double d1, double d2);
        [OperationContract]
        double Multiply(double d1, double d2);
        [OperationContract]
        double Divide(double d1, double d2);
    }
}
