using System.Globalization;
using System.ServiceModel;
using WCFSample.Contract;

namespace WCFSample.Console.Duplex.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CalculatorDuplexService:ICalculatorDuplex
    {
        private double _result;
        private string _equation;
        private readonly ICalculatorDuplexCallback _callback;

        public CalculatorDuplexService()
        {
            _result = 0.0D;
            _equation = _result.ToString(CultureInfo.InvariantCulture);
            _callback = OperationContext.Current.GetCallbackChannel<ICalculatorDuplexCallback>();
        }
        
        public void Clear()
        {
            _callback.Equation(_equation+"="+_result.ToString(CultureInfo.InvariantCulture));
            _result = 0.0D;
            _equation = _result.ToString(CultureInfo.InvariantCulture);
        }

        public void AddTo(double n)
        {
            _result += n;
            _equation += " + " + n.ToString(CultureInfo.InvariantCulture);
            _callback.Equals(_result);
        }

        public void SubtractFrom(double n)
        {
            _result -= n;
            _equation += " - " + n.ToString(CultureInfo.InvariantCulture);
            _callback.Equals(_result);
        }

        public void MuliplyBy(double n)
        {
            _result *= n;
            _equation += " * " + n.ToString(CultureInfo.InvariantCulture);
            _callback.Equals(_result);
        }

        public void DivideBy(double n)
        {
            _result /= n;
            _equation += " / " + n.ToString(CultureInfo.InvariantCulture);
            _callback.Equals(_result);
        }
    }
}
