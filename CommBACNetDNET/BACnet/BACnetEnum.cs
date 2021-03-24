using System;

namespace BACnet
{
    internal enum BACnetFunction : byte
    {
        BVLCResult = 0x00,
        WriteBroadcastDistributionTable = 0x01,
        ReadBroadcastDistributionTable = 0x02,
        ReadBroadcastDistributionTableACK = 0x03,
        ForwardedNPDU = 0x04,
        RegisterForeignDevice = 0x05,
        OriginalUnicastNPDU = 0x0a,
        OriginalBroadcastNPDU = 0x0b,
        SecureBVLL = 0x0c,
    }

    internal enum BACnetPDUType : byte
    {
        BACnetComfirmedRequestPDU = 0x00,
        BACnetUnconfirmedRequestPDU = 0x10,
        BACnetSimpleACKPDU = 0x20,
        BACnetComplexACKPDU = 0x30,
        SegmentACK = 0x40,
        ErrorPDU = 0x50,
        RejectPDU = 0x60,
        AbortPDU = 0x70,
        /// <summary>0x80 ~ 0xf0 Reserved
        /// </summary>
        Other = 0xf0,
    }

    internal enum BACnetObjectType
    {
        AI = 0,
        AO = 1,
        AV = 2,
        BI = 3,
        BO = 4,
        BV = 5,
        MSI = 13,
        MSO = 14,
        MSV = 19
    }

    internal enum BACnetErrorClass : byte
    {
        Device = 0,
        Object = 1,
        Property = 2,
        Resources = 3,
        Security = 4,
        Services = 5,
        VT = 6,
    }

    internal enum BACnetErrorCode : byte
    {
        Other = 0,
        AuthenticationFailed = 1,
        ConfigurationInProgress = 2,
        DeviceBusy = 3,
        DynamicCreationNotSupported = 4,
        FileAccessDenied = 5,
        IncompatibleSercurityLevels = 6,
        InconsistentParameters = 7,
        InconsistentSelectionCriterion = 8,
        InvalidDataType = 9,
        InvalidFileAccessMethod = 10,
        InvalidFileStartPosition = 11,
        InvalidOperatorName = 12,
        InvalidParameterDataType = 13,
        InvalidTimeStamp = 14,
        KeyGenerationError = 15,
        MissingRequiredParameter = 16,
        NoObjectsOfSpecifiedType = 17,
        NoSpaceForObject = 18,
        NoSpaceToAddListElement = 19,
        NoSpaceToWriteProperty = 20,
        NoVTSessionsAvailable = 21,
        PropertyIsNoAList = 22,
        ObjectDeleteNotPermitted = 23,
        ObjectIdentifierAlreadyExists = 24,
        OperationalProblem = 25,
        PasswordFailure = 26,
        ReadAccessDenied = 27,
        SecurityNotSupported = 28,
        ServiceRequestDenied = 29,
        Timeout = 30,
        UnknownObject = 31,
        UnknownProperty = 32,
        UnknownVTClass = 34,
        UnknownVTSession = 35,
        UnsupportedObjectType = 36,
        ValueOutOfRange = 37,
        VTSessionAlreadyCloses = 38,
        VTSessionTerminationFailure = 39,
        WriteAccessDenied = 40,
        CharaterSetNotSupported = 41,
        InvalidIndexOrArrayIndex = 42,
    }

    internal enum BACnetRejectReason
    {
        Other = 0,
        BufferOverflow = 1,
        InconsistentParameters = 2,
        InvalidParameterDataType = 3,
        InvalidTag = 4,
        MissingRequiredParameter = 5,
        ParameterOutOfRange = 6,
        TooManyArguments = 7,
        UndefinedEnumeration = 8,
        UnrecongnizedService = 9,
    }

    internal enum BACnetService
    {
        IAm = 0,
        IHave = 1,
        WhoHas = 2,
        WhoIs = 8,
        SubscribeCOV = 0x05,
        ReadProperty = 0x0c,
        ReadPropertyMultiple = 0x0e,
        WriteProperty = 0x0f,
        WritePropertyMultiple = 0x10,
        DeviceCommunicationControl = 0x11,
        ReinitializeDevice = 0x14,
    }
}
