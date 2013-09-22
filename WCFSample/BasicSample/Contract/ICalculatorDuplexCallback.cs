using System.ServiceModel;

namespace WCFSample.Contract
{
    public interface ICalculatorDuplexCallback
    {
        [OperationContract(IsOneWay = true)]
        void Equals(double result);

        [OperationContract(IsOneWay = true)]
        void Equation(string eqn);
    }
}