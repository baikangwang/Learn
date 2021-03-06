﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace WCFSample.Contract
{
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples",SessionMode = SessionMode.Required, CallbackContract = typeof(ICalculatorDuplexCallback))]
    public interface ICalculatorDuplex
    {
        [OperationContract(IsOneWay = true)]
        void Clear();
        [OperationContract(IsOneWay = true)]
        void AddTo(double n);
        [OperationContract(IsOneWay = true)]
        void SubtractFrom(double n);
        [OperationContract(IsOneWay = true)]
        void MuliplyBy(double n);
        [OperationContract(IsOneWay = true)]
        void DivideBy(double n);
    }
}
