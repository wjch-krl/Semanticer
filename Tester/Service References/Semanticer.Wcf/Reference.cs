﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tester.Semanticer.Wcf {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="SemanticResult", Namespace="http://schemas.datacontract.org/2004/07/Semanticer")]
    [System.SerializableAttribute()]
    public partial class SemanticResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double Propabilityk__BackingFieldField;
        
        private Tester.Semanticer.Wcf.PostMarkType Resultk__BackingFieldField;
        
        private string Textk__BackingFieldField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<Propability>k__BackingField", IsRequired=true)]
        public double Propabilityk__BackingField {
            get {
                return this.Propabilityk__BackingFieldField;
            }
            set {
                if ((this.Propabilityk__BackingFieldField.Equals(value) != true)) {
                    this.Propabilityk__BackingFieldField = value;
                    this.RaisePropertyChanged("Propabilityk__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<Result>k__BackingField", IsRequired=true)]
        public Tester.Semanticer.Wcf.PostMarkType Resultk__BackingField {
            get {
                return this.Resultk__BackingFieldField;
            }
            set {
                if ((this.Resultk__BackingFieldField.Equals(value) != true)) {
                    this.Resultk__BackingFieldField = value;
                    this.RaisePropertyChanged("Resultk__BackingField");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<Text>k__BackingField", IsRequired=true)]
        public string Textk__BackingField {
            get {
                return this.Textk__BackingFieldField;
            }
            set {
                if ((object.ReferenceEquals(this.Textk__BackingFieldField, value) != true)) {
                    this.Textk__BackingFieldField = value;
                    this.RaisePropertyChanged("Textk__BackingField");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PostMarkType", Namespace="http://schemas.datacontract.org/2004/07/Semanticer.Common.Enums")]
    public enum PostMarkType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NonCaluculated = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Positive = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Neutral = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Negative = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NonSupportedLanguage = -1,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Semanticer.Wcf.ISemanticProccessorService")]
    public interface ISemanticProccessorService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISemanticProccessorService/Process", ReplyAction="http://tempuri.org/ISemanticProccessorService/ProcessResponse")]
        Tester.Semanticer.Wcf.SemanticResult Process(string toEvaluate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISemanticProccessorService/Process", ReplyAction="http://tempuri.org/ISemanticProccessorService/ProcessResponse")]
        System.Threading.Tasks.Task<Tester.Semanticer.Wcf.SemanticResult> ProcessAsync(string toEvaluate);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISemanticProccessorServiceChannel : Tester.Semanticer.Wcf.ISemanticProccessorService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SemanticProccessorServiceClient : System.ServiceModel.ClientBase<Tester.Semanticer.Wcf.ISemanticProccessorService>, Tester.Semanticer.Wcf.ISemanticProccessorService {
        
        public SemanticProccessorServiceClient() {
        }
        
        public SemanticProccessorServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SemanticProccessorServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SemanticProccessorServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SemanticProccessorServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Tester.Semanticer.Wcf.SemanticResult Process(string toEvaluate) {
            return base.Channel.Process(toEvaluate);
        }
        
        public System.Threading.Tasks.Task<Tester.Semanticer.Wcf.SemanticResult> ProcessAsync(string toEvaluate) {
            return base.Channel.ProcessAsync(toEvaluate);
        }
    }
}