﻿//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMART.WCS.UI.COMMON.DeployWebService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DeployServerFileInfo", Namespace="http://schemas.datacontract.org/2004/07/SMART.WCS.WebService.LiveUpdate")]
    [System.SerializableAttribute()]
    public partial class DeployServerFileInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, object>[] ServerFileInfoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResultCDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResultMsgField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, object>[] ServerFileInfo {
            get {
                return this.ServerFileInfoField;
            }
            set {
                if ((object.ReferenceEquals(this.ServerFileInfoField, value) != true)) {
                    this.ServerFileInfoField = value;
                    this.RaisePropertyChanged("ServerFileInfo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public string ResultCD {
            get {
                return this.ResultCDField;
            }
            set {
                if ((object.ReferenceEquals(this.ResultCDField, value) != true)) {
                    this.ResultCDField = value;
                    this.RaisePropertyChanged("ResultCD");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public string ResultMsg {
            get {
                return this.ResultMsgField;
            }
            set {
                if ((object.ReferenceEquals(this.ResultMsgField, value) != true)) {
                    this.ResultMsgField = value;
                    this.RaisePropertyChanged("ResultMsg");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CompositeType", Namespace="http://schemas.datacontract.org/2004/07/SMART.WCS.WebService.LiveUpdate")]
    [System.SerializableAttribute()]
    public partial class CompositeType : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool BoolValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StringValueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BoolValue {
            get {
                return this.BoolValueField;
            }
            set {
                if ((this.BoolValueField.Equals(value) != true)) {
                    this.BoolValueField = value;
                    this.RaisePropertyChanged("BoolValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StringValue {
            get {
                return this.StringValueField;
            }
            set {
                if ((object.ReferenceEquals(this.StringValueField, value) != true)) {
                    this.StringValueField = value;
                    this.RaisePropertyChanged("StringValue");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DeployWebService.ILiveUpdateWebService")]
    public interface ILiveUpdateWebService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetServerDeployList", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetServerDeployListResponse")]
        System.Collections.Generic.Dictionary<string, string[]> GetServerDeployList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetServerDeployList", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetServerDeployListResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string[]>> GetServerDeployListAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/CompressionBackupFile", ReplyAction="http://tempuri.org/ILiveUpdateWebService/CompressionBackupFileResponse")]
        SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileResponse CompressionBackupFile(SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileRequest request);
        
        // CODEGEN: 작업에 여러 개의 반환 값이 있기 때문에 메시지 계약을 생성하는 중입니다.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/CompressionBackupFile", ReplyAction="http://tempuri.org/ILiveUpdateWebService/CompressionBackupFileResponse")]
        System.Threading.Tasks.Task<SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileResponse> CompressionBackupFileAsync(SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/CheckServerDeployDirectory", ReplyAction="http://tempuri.org/ILiveUpdateWebService/CheckServerDeployDirectoryResponse")]
        bool CheckServerDeployDirectory();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/CheckServerDeployDirectory", ReplyAction="http://tempuri.org/ILiveUpdateWebService/CheckServerDeployDirectoryResponse")]
        System.Threading.Tasks.Task<bool> CheckServerDeployDirectoryAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetDeployServerFileInfo", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetDeployServerFileInfoResponse")]
        SMART.WCS.UI.COMMON.DeployWebService.DeployServerFileInfo GetDeployServerFileInfo(string _strAppName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetDeployServerFileInfo", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetDeployServerFileInfoResponse")]
        System.Threading.Tasks.Task<SMART.WCS.UI.COMMON.DeployWebService.DeployServerFileInfo> GetDeployServerFileInfoAsync(string _strAppName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetData", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetDataResponse")]
        string GetData(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetData", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetDataResponse")]
        System.Threading.Tasks.Task<string> GetDataAsync(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetDataUsingDataContractResponse")]
        SMART.WCS.UI.COMMON.DeployWebService.CompositeType GetDataUsingDataContract(SMART.WCS.UI.COMMON.DeployWebService.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILiveUpdateWebService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/ILiveUpdateWebService/GetDataUsingDataContractResponse")]
        System.Threading.Tasks.Task<SMART.WCS.UI.COMMON.DeployWebService.CompositeType> GetDataUsingDataContractAsync(SMART.WCS.UI.COMMON.DeployWebService.CompositeType composite);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CompressionBackupFile", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class CompressionBackupFileRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string _strBackupFileName;
        
        public CompressionBackupFileRequest() {
        }
        
        public CompressionBackupFileRequest(string _strBackupFileName) {
            this._strBackupFileName = _strBackupFileName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CompressionBackupFileResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class CompressionBackupFileResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool CompressionBackupFileResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string _strBackupFileName;
        
        public CompressionBackupFileResponse() {
        }
        
        public CompressionBackupFileResponse(bool CompressionBackupFileResult, string _strBackupFileName) {
            this.CompressionBackupFileResult = CompressionBackupFileResult;
            this._strBackupFileName = _strBackupFileName;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILiveUpdateWebServiceChannel : SMART.WCS.UI.COMMON.DeployWebService.ILiveUpdateWebService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LiveUpdateWebServiceClient : System.ServiceModel.ClientBase<SMART.WCS.UI.COMMON.DeployWebService.ILiveUpdateWebService>, SMART.WCS.UI.COMMON.DeployWebService.ILiveUpdateWebService {
        
        public LiveUpdateWebServiceClient() {
        }
        
        public LiveUpdateWebServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LiveUpdateWebServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LiveUpdateWebServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LiveUpdateWebServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Collections.Generic.Dictionary<string, string[]> GetServerDeployList() {
            return base.Channel.GetServerDeployList();
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string[]>> GetServerDeployListAsync() {
            return base.Channel.GetServerDeployListAsync();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileResponse SMART.WCS.UI.COMMON.DeployWebService.ILiveUpdateWebService.CompressionBackupFile(SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileRequest request) {
            return base.Channel.CompressionBackupFile(request);
        }
        
        public bool CompressionBackupFile(ref string _strBackupFileName) {
            SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileRequest inValue = new SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileRequest();
            inValue._strBackupFileName = _strBackupFileName;
            SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileResponse retVal = ((SMART.WCS.UI.COMMON.DeployWebService.ILiveUpdateWebService)(this)).CompressionBackupFile(inValue);
            _strBackupFileName = retVal._strBackupFileName;
            return retVal.CompressionBackupFileResult;
        }
        
        public System.Threading.Tasks.Task<SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileResponse> CompressionBackupFileAsync(SMART.WCS.UI.COMMON.DeployWebService.CompressionBackupFileRequest request) {
            return base.Channel.CompressionBackupFileAsync(request);
        }
        
        public bool CheckServerDeployDirectory() {
            return base.Channel.CheckServerDeployDirectory();
        }
        
        public System.Threading.Tasks.Task<bool> CheckServerDeployDirectoryAsync() {
            return base.Channel.CheckServerDeployDirectoryAsync();
        }
        
        public SMART.WCS.UI.COMMON.DeployWebService.DeployServerFileInfo GetDeployServerFileInfo(string _strAppName) {
            return base.Channel.GetDeployServerFileInfo(_strAppName);
        }
        
        public System.Threading.Tasks.Task<SMART.WCS.UI.COMMON.DeployWebService.DeployServerFileInfo> GetDeployServerFileInfoAsync(string _strAppName) {
            return base.Channel.GetDeployServerFileInfoAsync(_strAppName);
        }
        
        public string GetData(int value) {
            return base.Channel.GetData(value);
        }
        
        public System.Threading.Tasks.Task<string> GetDataAsync(int value) {
            return base.Channel.GetDataAsync(value);
        }
        
        public SMART.WCS.UI.COMMON.DeployWebService.CompositeType GetDataUsingDataContract(SMART.WCS.UI.COMMON.DeployWebService.CompositeType composite) {
            return base.Channel.GetDataUsingDataContract(composite);
        }
        
        public System.Threading.Tasks.Task<SMART.WCS.UI.COMMON.DeployWebService.CompositeType> GetDataUsingDataContractAsync(SMART.WCS.UI.COMMON.DeployWebService.CompositeType composite) {
            return base.Channel.GetDataUsingDataContractAsync(composite);
        }
    }
}