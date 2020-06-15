﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pakerator.ServiceReferenceIAI {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="CheckServerLoad", ConfigurationName="ServiceReferenceIAI.checkServerLoadPortType")]
    public interface checkServerLoadPortType {
        
        [System.ServiceModel.OperationContractAttribute(Action="#checkServerLoad", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="checkServerLoadResponse")]
        Pakerator.ServiceReferenceIAI.responseType checkServerLoad(Pakerator.ServiceReferenceIAI.requestType checkServerLoadRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="#checkServerLoad", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="checkServerLoadResponse")]
        System.Threading.Tasks.Task<Pakerator.ServiceReferenceIAI.responseType> checkServerLoadAsync(Pakerator.ServiceReferenceIAI.requestType checkServerLoadRequest);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="CheckServerLoad")]
    public partial class requestType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private authenticateType authenticateField;
        
        /// <remarks/>
        public authenticateType authenticate {
            get {
                return this.authenticateField;
            }
            set {
                this.authenticateField = value;
                this.RaisePropertyChanged("authenticate");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="CheckServerLoad")]
    public partial class authenticateType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string system_keyField;
        
        private string system_loginField;
        
        /// <remarks/>
        public string system_key {
            get {
                return this.system_keyField;
            }
            set {
                this.system_keyField = value;
                this.RaisePropertyChanged("system_key");
            }
        }
        
        /// <remarks/>
        public string system_login {
            get {
                return this.system_loginField;
            }
            set {
                this.system_loginField = value;
                this.RaisePropertyChanged("system_login");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="CheckServerLoad")]
    public partial class errorsType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int faultCodeField;
        
        private bool faultCodeFieldSpecified;
        
        private string faultStringField;
        
        /// <remarks/>
        public int faultCode {
            get {
                return this.faultCodeField;
            }
            set {
                this.faultCodeField = value;
                this.RaisePropertyChanged("faultCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapIgnoreAttribute()]
        public bool faultCodeSpecified {
            get {
                return this.faultCodeFieldSpecified;
            }
            set {
                this.faultCodeFieldSpecified = value;
                this.RaisePropertyChanged("faultCodeSpecified");
            }
        }
        
        /// <remarks/>
        public string faultString {
            get {
                return this.faultStringField;
            }
            set {
                this.faultStringField = value;
                this.RaisePropertyChanged("faultString");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="CheckServerLoad")]
    public partial class responseType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private authenticateType authenticateField;
        
        private errorsType errorsField;
        
        private serverLoadStatusType serverLoadStatusField;
        
        private bool serverLoadStatusFieldSpecified;
        
        /// <remarks/>
        public authenticateType authenticate {
            get {
                return this.authenticateField;
            }
            set {
                this.authenticateField = value;
                this.RaisePropertyChanged("authenticate");
            }
        }
        
        /// <remarks/>
        public errorsType errors {
            get {
                return this.errorsField;
            }
            set {
                this.errorsField = value;
                this.RaisePropertyChanged("errors");
            }
        }
        
        /// <remarks/>
        public serverLoadStatusType serverLoadStatus {
            get {
                return this.serverLoadStatusField;
            }
            set {
                this.serverLoadStatusField = value;
                this.RaisePropertyChanged("serverLoadStatus");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapIgnoreAttribute()]
        public bool serverLoadStatusSpecified {
            get {
                return this.serverLoadStatusFieldSpecified;
            }
            set {
                this.serverLoadStatusFieldSpecified = value;
                this.RaisePropertyChanged("serverLoadStatusSpecified");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="CheckServerLoad")]
    public enum serverLoadStatusType {
        
        /// <remarks/>
        overloaded,
        
        /// <remarks/>
        normal,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface checkServerLoadPortTypeChannel : Pakerator.ServiceReferenceIAI.checkServerLoadPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class checkServerLoadPortTypeClient : System.ServiceModel.ClientBase<Pakerator.ServiceReferenceIAI.checkServerLoadPortType>, Pakerator.ServiceReferenceIAI.checkServerLoadPortType {
        
        public checkServerLoadPortTypeClient() {
        }
        
        public checkServerLoadPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public checkServerLoadPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public checkServerLoadPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public checkServerLoadPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Pakerator.ServiceReferenceIAI.responseType checkServerLoad(Pakerator.ServiceReferenceIAI.requestType checkServerLoadRequest) {
            return base.Channel.checkServerLoad(checkServerLoadRequest);
        }
        
        public System.Threading.Tasks.Task<Pakerator.ServiceReferenceIAI.responseType> checkServerLoadAsync(Pakerator.ServiceReferenceIAI.requestType checkServerLoadRequest) {
            return base.Channel.checkServerLoadAsync(checkServerLoadRequest);
        }
    }
}
