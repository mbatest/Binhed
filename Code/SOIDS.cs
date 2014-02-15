using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code
{
    public static class SOID_KEYS
    {
        public static SortedList<string, string> keys;
        public static void SetKeys()
        {
            keys = new SortedList<string, string>();
            try
            {
                keys.Add("0.0", "");
                keys.Add("0.9.2342.19200300.100.1.25", "DOMAIN_COMPONENT");
                #region 1.2
                keys.Add("1.2.840.10040", "X957");
                keys.Add("1.2.840.10040.4.1", "X957_DSA");
                keys.Add("1.2.840.10040.4.3", "X957_SHA1DSA");
                keys.Add("1.2.840.10046", "ANSI_X942");
                keys.Add("1.2.840.10046.2.1", "ANSI_X942_DH");
                keys.Add("1.2.840.113549", "RSA");
                keys.Add("1.2.840.113549.1", "PKCS");
                keys.Add("1.2.840.113549.1.1", "PKCS_1");
                keys.Add("1.2.840.113549.1.1.1", "RSA_RSA");
                keys.Add("1.2.840.113549.1.1.2", "RSA_MD2RSA");
                keys.Add("1.2.840.113549.1.1.3", "RSA_MD4RSA");
                keys.Add("1.2.840.113549.1.1.4", "RSA_MD5RSA");
                keys.Add("1.2.840.113549.1.1.5", "RSA_SHA1RSA");
                keys.Add("1.2.840.113549.1.1.6", "RSA_SET0AEP_RSA");
                keys.Add("1.2.840.113549.1.10", "PKCS_10");
                keys.Add("1.2.840.113549.1.12", "PKCS_11");
                keys.Add("1.2.840.113549.1.2", "PKCS_2");
                keys.Add("1.2.840.113549.1.3", "PKCS_3");
                keys.Add("1.2.840.113549.1.3.1", "RSA_DH");
                keys.Add("1.2.840.113549.1.4", "PKCS_4");
                keys.Add("1.2.840.113549.1.5", "PKCS_5");
                keys.Add("1.2.840.113549.1.6", "PKCS_6");
                keys.Add("1.2.840.113549.1.7", "PKCS_7");
                keys.Add("1.2.840.113549.1.7.1", "RSA_data");
                keys.Add("1.2.840.113549.1.7.2", "RSA_signedData");
                keys.Add("1.2.840.113549.1.7.3", "RSA_envelopedData");
                keys.Add("1.2.840.113549.1.7.4", "RSA_signEnvData");
                keys.Add("1.2.840.113549.1.7.5", "RSA_digestedData");
                keys.Add("1.2.840.113549.1.7.6", "RSA_encryptedData");
                keys.Add("1.2.840.113549.1.8", "PKCS_8");
                keys.Add("1.2.840.113549.1.9", "PKCS_9");
                keys.Add("1.2.840.113549.1.9.1", "RSA_emailAddr");
                keys.Add("1.2.840.113549.1.9.14", "RSA_certExtensions");
                keys.Add("1.2.840.113549.1.9.15", "RSA_SMIMECapabilities");
                keys.Add("1.2.840.113549.1.9.15.1", "RSA_preferSignedData");
                keys.Add("1.2.840.113549.1.9.16.3", "RSA_SMIMEalg");
                keys.Add("1.2.840.113549.1.9.16.3.5", "RSA_SMIMEalgESDH");
                keys.Add("1.2.840.113549.1.9.16.3.6", "RSA_SMIMEalgCMS3DESwrap");
                keys.Add("1.2.840.113549.1.9.16.3.7", "RSA_SMIMEalgCMSRC2wrap");
                keys.Add("1.2.840.113549.1.9.2", "RSA_unstructName");
                keys.Add("1.2.840.113549.1.9.20", "PKCS_12_FRIENDLY_NAME_ATTR");
                keys.Add("1.2.840.113549.1.9.21", "PKCS_12_LOCAL_KEY_ID");
                keys.Add("1.2.840.113549.1.9.3", "RSA_contentType");
                keys.Add("1.2.840.113549.1.9.4", "RSA_messageDigest");
                keys.Add("1.2.840.113549.1.9.5", "RSA_signingTime");
                keys.Add("1.2.840.113549.1.9.6", "RSA_counterSign");
                keys.Add("1.2.840.113549.1.9.7", "RSA_challengePwd");
                keys.Add("1.2.840.113549.1.9.9", "RSA_extCertAttrs");
                keys.Add("1.2.840.113549.2", "RSA_HASH");
                keys.Add("1.2.840.113549.2.2", "RSA_MD2");
                keys.Add("1.2.840.113549.2.4", "RSA_MD4");
                keys.Add("1.2.840.113549.2.5", "RSA_MD5");
                keys.Add("1.2.840.113549.3", "RSA_ENCRYPT");
                keys.Add("1.2.840.113549.3.2", "RSA_RC2CBC");
                keys.Add("1.2.840.113549.3.4", "RSA_RC4");
                keys.Add("1.2.840.113549.3.7", "RSA_DES_EDE3_CBC");
                keys.Add("1.2.840.113549.3.9", "RSA_RC5_CBCPad");
                #endregion
                #region 1.3.14 OIW
                keys.Add("1.3.14", "OIW");
                keys.Add("1.3.14.3.2", "OIWSEC");
                keys.Add("1.3.14.3.2.10", "OIWSEC_desMAC");
                keys.Add("1.3.14.3.2.11", "OIWSEC_rsaSign");
                keys.Add("1.3.14.3.2.12", "OIWSEC_dsa");
                keys.Add("1.3.14.3.2.13", "OIWSEC_shaDSA");
                keys.Add("1.3.14.3.2.14", "OIWSEC_mdc2RSA");
                keys.Add("1.3.14.3.2.15", "OIWSEC_shaRSA");
                keys.Add("1.3.14.3.2.16", "OIWSEC_dhCommMod");
                keys.Add("1.3.14.3.2.17", "OIWSEC_desEDE");
                keys.Add("1.3.14.3.2.18", "OIWSEC_sha");
                keys.Add("1.3.14.3.2.19", "OIWSEC_mdc2");
                keys.Add("1.3.14.3.2.2", "OIWSEC_md4RSA");
                keys.Add("1.3.14.3.2.20", "OIWSEC_dsaComm");
                keys.Add("1.3.14.3.2.21", "OIWSEC_dsaCommSHA");
                keys.Add("1.3.14.3.2.22", "OIWSEC_rsaXchg");
                keys.Add("1.3.14.3.2.23", "OIWSEC_keyHashSeal");
                keys.Add("1.3.14.3.2.24", "OIWSEC_md2RSASign");
                keys.Add("1.3.14.3.2.25", "OIWSEC_md5RSASign");
                keys.Add("1.3.14.3.2.26", "OIWSEC_sha1");
                keys.Add("1.3.14.3.2.27", "OIWSEC_dsaSHA1");
                keys.Add("1.3.14.3.2.28", "OIWSEC_dsaCommSHA1");
                keys.Add("1.3.14.3.2.29", "OIWSEC_sha1RSASign");
                keys.Add("1.3.14.3.2.3", "OIWSEC_md5RSA");
                keys.Add("1.3.14.3.2.4", "OIWSEC_md4RSA2");
                keys.Add("1.3.14.3.2.6", "OIWSEC_desECB");
                keys.Add("1.3.14.3.2.7", "OIWSEC_desCBC");
                keys.Add("1.3.14.3.2.8", "OIWSEC_desOFB");
                keys.Add("1.3.14.3.2.9", "OIWSEC_desCFB");
                keys.Add("1.3.14.7.2", "OIWDIR");
                keys.Add("1.3.14.7.2.1", "OIWDIR_CRPT");
                keys.Add("1.3.14.7.2.2", "OIWDIR_HASH");
                keys.Add("1.3.14.7.2.2.1", "OIWDIR_md2");
                keys.Add("1.3.14.7.2.3", "OIWDIR_SIGN");
                keys.Add("1.3.14.7.2.3.1", "OIWDIR_md2RSA");
                #endregion
                #region 1.3.6.1.4.1.311 : Microsoft
                keys.Add("1.3.6.1.4.1.311", "Microsoft OID");
                keys.Add("1.3.6.1.4.1.311.10", "Crypto 2.0");
                keys.Add("1.3.6.1.4.1.311.10.1", "CTL");
                keys.Add("1.3.6.1.4.1.311.10.1.1", "SORTED_CTL");
                keys.Add("1.3.6.1.4.1.311.10.10", "Microsoft CMC OIDs");
                keys.Add("1.3.6.1.4.1.311.10.10.1", "CMC_ADD_ATTRIBUTES");
                keys.Add("1.3.6.1.4.1.311.10.10.1.1", "SORTED_CTL");
                keys.Add("1.3.6.1.4.1.311.10.11", "Microsoft certificate property OIDs");
                keys.Add("1.3.6.1.4.1.311.10.11.11", "CERT_PROP_ID_PREFIX");
                keys.Add("1.3.6.1.4.1.311.10.12", "CryptUI");
                keys.Add("1.3.6.1.4.1.311.10.12.1", "ANY_APPLICATION_POLICY");
                keys.Add("1.3.6.1.4.1.311.10.2", "NEXT_UPDATE_LOCATION");
                keys.Add("1.3.6.1.4.1.311.10.3.1", "KP_CTL_USAGE_SIGNING");
                keys.Add("1.3.6.1.4.1.311.10.3.10", "KP_QUALIFIED_SUBORDINATION");
                keys.Add("1.3.6.1.4.1.311.10.3.11", "KP_KEY_RECOVERY");
                keys.Add("1.3.6.1.4.1.311.10.3.12", "KP_DOCUMENT_SIGNING");
                keys.Add("1.3.6.1.4.1.311.10.3.13", "KP_LIFETIME_SIGNING");
                keys.Add("1.3.6.1.4.1.311.10.3.14", "KP_MOBILE_DEVICE_SOFTWARE");
                keys.Add("1.3.6.1.4.1.311.10.3.2", "KP_TIME_STAMP_SIGNING");
                keys.Add("1.3.6.1.4.1.311.10.3.3", "SERVER_GATED_CRYPTO");
                keys.Add("1.3.6.1.4.1.311.10.3.3.1", "SERIALIZED");
                keys.Add("1.3.6.1.4.1.311.10.3.4", "KP_EFS");
                keys.Add("1.3.6.1.4.1.311.10.3.4.1", "EFS_RECOVERY");
                keys.Add("1.3.6.1.4.1.311.10.3.5", "WHQL_CRYPTO");
                keys.Add("1.3.6.1.4.1.311.10.3.6", "NT5_CRYPTO");
                keys.Add("1.3.6.1.4.1.311.10.3.7", "OEM_WHQL_CRYPTO");
                keys.Add("1.3.6.1.4.1.311.10.3.8", "EMBEDDED_NT_CRYPTO");
                keys.Add("1.3.6.1.4.1.311.10.3.9", "ROOT_LIST_SIGNER");
                keys.Add("1.3.6.1.4.1.311.10.4.1", "YESNO_TRUST_ATTR");
                keys.Add("1.3.6.1.4.1.311.10.5.1", "DRM");
                keys.Add("1.3.6.1.4.1.311.10.5.2", "DRM_INDIVIDUALIZATION");
                keys.Add("1.3.6.1.4.1.311.10.6.1", "LICENSES");
                keys.Add("1.3.6.1.4.1.311.10.6.2", "LICENSE_SERVER");
                keys.Add("1.3.6.1.4.1.311.10.7", "MICROSOFT_RDN_PREFIX");
                keys.Add("1.3.6.1.4.1.311.10.7.1", "KEYID_RDN");
                keys.Add("1.3.6.1.4.1.311.10.8.1", "REMOVE_CERTIFICATE");
                keys.Add("1.3.6.1.4.1.311.10.9.1", "CROSS_CERT_DIST_POINTS");
                keys.Add("1.3.6.1.4.1.311.12", "Catalog");
                keys.Add("1.3.6.1.4.1.311.12.1.1", "CATALOG_LIST");
                keys.Add("1.3.6.1.4.1.311.12.1.2", "CATALOG_LIST_MEMBER");
                keys.Add("1.3.6.1.4.1.311.12.2.1", "CAT_NAMEVALUE_OBJID");
                keys.Add("1.3.6.1.4.1.311.12.2.2", "CAT_MEMBERINFO_OBJID");
                keys.Add("1.3.6.1.4.1.311.13", "Microsoft PKCS10 OIDs");
                keys.Add("1.3.6.1.4.1.311.13.1", "RENEWAL_CERTIFICATE");
                keys.Add("1.3.6.1.4.1.311.13.2.1", "ENROLLMENT_NAME_VALUE_PAIR");
                keys.Add("1.3.6.1.4.1.311.13.2.2", "ENROLLMENT_CSP_PROVIDER");
                keys.Add("1.3.6.1.4.1.311.13.2.3", "OS_VERSION");
                keys.Add("1.3.6.1.4.1.311.15", "Microsoft Java");
                keys.Add("1.3.6.1.4.1.311.16", "Microsoft Outlook/Exchange");
                keys.Add("1.3.6.1.4.1.311.16.4", "Outlook Express");
                keys.Add("1.3.6.1.4.1.311.17", "Microsoft PKCS12 attributes");
                keys.Add("1.3.6.1.4.1.311.17.1", "PKCS_12_KEY_PROVIDER_NAME_ATTR");
                keys.Add("1.3.6.1.4.1.311.17.2", "LOCAL_MACHINE_KEYSET");
                keys.Add("1.3.6.1.4.1.311.18", "Microsoft Hydra");
                keys.Add("1.3.6.1.4.1.311.18.1", "PKIX_LICENSE_INFO");
                keys.Add("1.3.6.1.4.1.311.18.2", "PKIX_MANUFACTURER");
                keys.Add("1.3.6.1.4.1.311.18.4", "PKIX_HYDRA_CERT_VERSION");
                keys.Add("1.3.6.1.4.1.311.18.5", "PKIX_LICENSED_PRODUCT_INFO");
                keys.Add("1.3.6.1.4.1.311.18.6", "PKIX_MS_LICENSE_SERVER_INFO");
                keys.Add("1.3.6.1.4.1.311.18.7", "PKIS_PRODUCT_SPECIFIC_OID");
                keys.Add("1.3.6.1.4.1.311.18.8", "PKIS_TLSERVER_SPK_OID");
                keys.Add("1.3.6.1.4.1.311.19", "Microsoft ISPU Test");
                keys.Add("1.3.6.1.4.1.311.2", "Authenticode");
                keys.Add("1.3.6.1.4.1.311.2.1.10", "SPC_SP_AGENCY_INFO_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.11", "SPC_STATEMENT_TYPE_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.12", "SPC_SP_OPUS_INFO_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.14", "SPC_CERT_EXTENSIONS_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.15", "SPC_PE_IMAGE_DATA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.18", "SPC_RAW_FILE_DATA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.19", "SPC_STRUCTURED_STORAGE_DATA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.20", "SPC_JAVA_CLASS_DATA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.21", "SPC_INDIVIDUAL_SP_KEY_PURPOSE_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.22", "SPC_COMMERCIAL_SP_KEY_PURPOSE_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.25", "SPC_CAB_DATA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.26", "SPC_MINIMAL_CRITERIA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.27", "SPC_FINANCIAL_CRITERIA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.28", "SPC_LINK_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.29", "SPC_HASH_INFO_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.30", "SPC_SIPINFO_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.1.4", "SPC_INDIRECT_DATA_OBJID");
                keys.Add("1.3.6.1.4.1.311.2.2.1", "TRUSTED_CODESIGNING_CA_LIST");
                keys.Add("1.3.6.1.4.1.311.2.2.2", "TRUSTED_CLIENT_AUTH_CA_LIST");
                keys.Add("1.3.6.1.4.1.311.2.2.3", "TRUSTED_SERVER_AUTH_CA_LIST");
                keys.Add("1.3.6.1.4.1.311.20", "Microsoft Enrollment Infrastructure");
                keys.Add("1.3.6.1.4.1.311.20.1", "AUTO_ENROLL_CTL_USAGE");
                keys.Add("1.3.6.1.4.1.311.20.2", "ENROLL_CERTTYPE_EXTENSION");
                keys.Add("1.3.6.1.4.1.311.20.2.1", "ENROLLMENT_AGENT");
                keys.Add("1.3.6.1.4.1.311.20.2.2", "KP_SMARTCARD_LOGON");
                keys.Add("1.3.6.1.4.1.311.20.2.3", "NT_PRINCIPAL_NAME");
                keys.Add("1.3.6.1.4.1.311.20.3", "CERT_MANIFOLD");
                keys.Add("1.3.6.1.4.1.311.21", "Microsoft CertSrv Infrastructure");
                keys.Add("1.3.6.1.4.1.311.21.1", "CERTSRV_CA_VERSION");
                keys.Add("1.3.6.1.4.1.311.21.10", "APPLICATION_CERT_POLICIES");
                keys.Add("1.3.6.1.4.1.311.21.11", "APPLICATION_POLICY_MAPPINGS");
                keys.Add("1.3.6.1.4.1.311.21.12", "APPLICATION_POLICY_CONSTRAINTS");
                keys.Add("1.3.6.1.4.1.311.21.13", "ARCHIVED_KEY_ATTR");
                keys.Add("1.3.6.1.4.1.311.21.14", "CRL_SELF_CDP");
                keys.Add("1.3.6.1.4.1.311.21.15", "REQUIRE_CERT_CHAIN_POLICY");
                keys.Add("1.3.6.1.4.1.311.21.16", "ARCHIVED_KEY_CERT_HASH");
                keys.Add("1.3.6.1.4.1.311.21.17", "ISSUED_CERT_HASH");
                keys.Add("1.3.6.1.4.1.311.21.19", "DS_EMAIL_REPLICATION");
                keys.Add("1.3.6.1.4.1.311.21.2", "CERTSRV_PREVIOUS_CERT_HASH");
                keys.Add("1.3.6.1.4.1.311.21.20", "REQUEST_CLIENT_INFO");
                keys.Add("1.3.6.1.4.1.311.21.21", "ENCRYPTED_KEY_HASH");
                keys.Add("1.3.6.1.4.1.311.21.22", "CERTSRV_CROSSCA_VERSION");
                keys.Add("1.3.6.1.4.1.311.21.3", "CRL_VIRTUAL_BASE");
                keys.Add("1.3.6.1.4.1.311.21.4", "CRL_NEXT_PUBLISH");
                keys.Add("1.3.6.1.4.1.311.21.5", "KP_CA_EXCHANGE");
                keys.Add("1.3.6.1.4.1.311.21.6", "KP_KEY_RECOVERY_AGENT");
                keys.Add("1.3.6.1.4.1.311.21.7", "CERTIFICATE_TEMPLATE");
                keys.Add("1.3.6.1.4.1.311.21.8", "ENTERPRISE_OID_ROOT");
                keys.Add("1.3.6.1.4.1.311.21.9", "RDN_DUMMY_SIGNER");
                keys.Add("1.3.6.1.4.1.311.25", "Microsoft Directory Service");
                keys.Add("1.3.6.1.4.1.311.25.1", "NTDS_REPLICATION");
                keys.Add("1.3.6.1.4.1.311.3", "Time Stamping");
                keys.Add("1.3.6.1.4.1.311.3.2.1", "SPC_TIME_STAMP_REQUEST_OBJID");
                keys.Add("1.3.6.1.4.1.311.30", "IIS");
                keys.Add("1.3.6.1.4.1.311.30.1", "IIS_VIRTUAL_SERVER");
                keys.Add("1.3.6.1.4.1.311.31", "Windows updates and service packs");
                keys.Add("1.3.6.1.4.1.311.31.1", "PRODUCT_UPDATE");
                keys.Add("1.3.6.1.4.1.311.4", "Permissions");
                keys.Add("1.3.6.1.4.1.311.40", "Fonts");
                keys.Add("1.3.6.1.4.1.311.41", "Microsoft Licensing and Registration");
                keys.Add("1.3.6.1.4.1.311.42", "Microsoft Corporate PKI (ITG)");
                keys.Add("1.3.6.1.4.1.311.43", "Microsoft WWOps BizExt");
                keys.Add("1.3.6.1.4.1.311.44", "Microsoft Peer Networking");
                keys.Add("1.3.6.1.4.1.311.44.0.1", "PEERNET_CERT_TYPE");
                keys.Add("1.3.6.1.4.1.311.44.0.2", "PEERNET_PEERNAME");
                keys.Add("1.3.6.1.4.1.311.44.0.3", "PEERNET_CLASSIFIER");
                keys.Add("1.3.6.1.4.1.311.44.0.4", "PEERNET_CERT_VERSION");
                keys.Add("1.3.6.1.4.1.311.44.1", "PEERNET_PNRP");
                keys.Add("1.3.6.1.4.1.311.44.1.1", "PEERNET_PNRP_ADDRESS");
                keys.Add("1.3.6.1.4.1.311.44.1.2", "PEERNET_PNRP_FLAGS");
                keys.Add("1.3.6.1.4.1.311.44.1.3", "PEERNET_PNRP_PAYLOAD");
                keys.Add("1.3.6.1.4.1.311.44.1.4", "PEERNET_PNRP_ID");
                keys.Add("1.3.6.1.4.1.311.44.2", "PEERNET_IDENTITY");
                keys.Add("1.3.6.1.4.1.311.44.2.2", "PEERNET_IDENTITY_FLAGS");
                keys.Add("1.3.6.1.4.1.311.44.3", "PEERNET_GROUPING");
                keys.Add("1.3.6.1.4.1.311.44.3.1", "PEERNET_GROUPING_PEERNAME");
                keys.Add("1.3.6.1.4.1.311.44.3.2", "PEERNET_GROUPING_FLAGS");
                keys.Add("1.3.6.1.4.1.311.44.3.3", "PEERNET_GROUPING_ROLES");
                keys.Add("1.3.6.1.4.1.311.44.3.5", "PEERNET_GROUPING_CLASSIFIERS");
                keys.Add("1.3.6.1.4.1.311.45", "Mobile Devices Code Signing");
                keys.Add("1.3.6.1.4.1.311.88", "CAPICOM");
                keys.Add("1.3.6.1.4.1.311.88.1", "CAPICOM_VERSION");
                keys.Add("1.3.6.1.4.1.311.88.2", "CAPICOM_ATTRIBUTE");
                keys.Add("1.3.6.1.4.1.311.88.2.1", "CAPICOM_DOCUMENT_NAME");
                keys.Add("1.3.6.1.4.1.311.88.2.2", "CAPICOM_DOCUMENT_DESCRIPTION");
                keys.Add("1.3.6.1.4.1.311.88.3", "CAPICOM_ENCRYPTED_DATA");
                keys.Add("1.3.6.1.4.1.311.88.3.1", "CAPICOM_ENCRYPTED_CONTENT");
                #endregion
                #region 1.3.6.1.5.5.7 PKIX
                keys.Add("1.3.6.1.5.5.7", "PKIX");
                keys.Add("1.3.6.1.5.5.7.1", "PKIX_PE");
                keys.Add("1.3.6.1.5.5.7.1.1", "AUTHORITY_INFO_ACCESS");
                keys.Add("1.3.6.1.5.5.7.12.2", "CT_PKI_DATA");
                keys.Add("1.3.6.1.5.5.7.12.3", "CT_PKI_RESPONSE");
                keys.Add("1.3.6.1.5.5.7.2.1", "PKIX_POLICY_QUALIFIER_CPS");
                keys.Add("1.3.6.1.5.5.7.2.2", "PKIX_POLICY_QUALIFIER_USERNOTICE");
                keys.Add("1.3.6.1.5.5.7.3", "PKIX_KP");
                keys.Add("1.3.6.1.5.5.7.3.1", "PKIX_KP_SERVER_AUTH");
                keys.Add("1.3.6.1.5.5.7.3.2", "PKIX_KP_CLIENT_AUTH");
                keys.Add("1.3.6.1.5.5.7.3.3", "PKIX_KP_CODE_SIGNING");
                keys.Add("1.3.6.1.5.5.7.3.4", "PKIX_KP_EMAIL_PROTECTION");
                keys.Add("1.3.6.1.5.5.7.3.5", "PKIX_KP_IPSEC_END_SYSTEM");
                keys.Add("1.3.6.1.5.5.7.3.6", "PKIX_KP_IPSEC_TUNNEL");
                keys.Add("1.3.6.1.5.5.7.3.7", "PKIX_KP_IPSEC_USER");
                keys.Add("1.3.6.1.5.5.7.3.8", "PKIX_KP_TIMESTAMP_SIGNING");
                keys.Add("1.3.6.1.5.5.7.48", "PKIX_ACC_DESCR");
                keys.Add("1.3.6.1.5.5.7.48.1", "PKIX_OCSP");
                keys.Add("1.3.6.1.5.5.7.48.2", "PKIX_CA_ISSUERS");
                keys.Add("1.3.6.1.5.5.7.6.2", "PKIX_NO_SIGNATURE");
                keys.Add("1.3.6.1.5.5.7.7", "CMC");
                keys.Add("1.3.6.1.5.5.7.7.1", "CMC_STATUS_INFO");
                keys.Add("1.3.6.1.5.5.7.7.10", "CMC_DECRYPTED_POP");
                keys.Add("1.3.6.1.5.5.7.7.11", "CMC_LRA_POP_WITNESS");
                keys.Add("1.3.6.1.5.5.7.7.15", "CMC_GET_CERT");
                keys.Add("1.3.6.1.5.5.7.7.16", "CMC_GET_CRL");
                keys.Add("1.3.6.1.5.5.7.7.17", "CMC_REVOKE_REQUEST");
                keys.Add("1.3.6.1.5.5.7.7.18", "CMC_REG_INFO");
                keys.Add("1.3.6.1.5.5.7.7.19", "CMC_RESPONSE_INFO");
                keys.Add("1.3.6.1.5.5.7.7.2", "CMC_IDENTIFICATION");
                keys.Add("1.3.6.1.5.5.7.7.21", "CMC_QUERY_PENDING");
                keys.Add("1.3.6.1.5.5.7.7.22", "CMC_ID_POP_LINK_RANDOM");
                keys.Add("1.3.6.1.5.5.7.7.23", "CMC_ID_POP_LINK_WITNESS");
                keys.Add("1.3.6.1.5.5.7.7.3", "CMC_IDENTITY_PROOF");
                keys.Add("1.3.6.1.5.5.7.7.4", "CMC_DATA_RETURN");
                keys.Add("1.3.6.1.5.5.7.7.5", "CMC_TRANSACTION_ID");
                keys.Add("1.3.6.1.5.5.7.7.6", "CMC_SENDER_NONCE");
                keys.Add("1.3.6.1.5.5.7.7.7", "CMC_RECIPIENT_NONCE");
                keys.Add("1.3.6.1.5.5.7.7.8", "CMC_ADD_EXTENSIONS");
                keys.Add("1.3.6.1.5.5.7.7.9", "CMC_ENCRYPTED_POP");
                #endregion
                keys.Add("1.3.6.1.5.5.8.2.2", "IPSEC_KP_IKE_INTERMEDIATE");
                #region 2.16
                keys.Add("2.16.840.1.101.2.1", "INFOSEC");
                keys.Add("2.16.840.1.101.2.1.1.1", "INFOSEC_sdnsSignature");
                keys.Add("2.16.840.1.101.2.1.1.10", "INFOSEC_mosaicKeyManagement");
                keys.Add("2.16.840.1.101.2.1.1.11", "INFOSEC_sdnsKMandSig");
                keys.Add("2.16.840.1.101.2.1.1.12", "INFOSEC_mosaicKMandSig");
                keys.Add("2.16.840.1.101.2.1.1.13", "INFOSEC_SuiteASignature");
                keys.Add("2.16.840.1.101.2.1.1.14", "INFOSEC_SuiteAConfidentiality");
                keys.Add("2.16.840.1.101.2.1.1.15", "INFOSEC_SuiteAIntegrity");
                keys.Add("2.16.840.1.101.2.1.1.16", "INFOSEC_SuiteATokenProtection");
                keys.Add("2.16.840.1.101.2.1.1.17", "INFOSEC_SuiteAKeyManagement");
                keys.Add("2.16.840.1.101.2.1.1.18", "INFOSEC_SuiteAKMandSig");
                keys.Add("2.16.840.1.101.2.1.1.19", "INFOSEC_mosaicUpdatedSig");
                keys.Add("2.16.840.1.101.2.1.1.2", "INFOSEC_mosaicSignature");
                keys.Add("2.16.840.1.101.2.1.1.20", "INFOSEC_mosaicKMandUpdSig");
                keys.Add("2.16.840.1.101.2.1.1.21", "INFOSEC_mosaicUpdateInteg");
                keys.Add("2.16.840.1.101.2.1.1.3", "INFOSEC_sdnsConfidentiality");
                keys.Add("2.16.840.1.101.2.1.1.4", "INFOSEC_mosaicConfidentiality");
                keys.Add("2.16.840.1.101.2.1.1.5", "INFOSEC_sdnsIntegrity");
                keys.Add("2.16.840.1.101.2.1.1.6", "INFOSEC_mosaicIntegrity");
                keys.Add("2.16.840.1.101.2.1.1.7", "INFOSEC_sdnsTokenProtection");
                keys.Add("2.16.840.1.101.2.1.1.8", "INFOSEC_mosaicTokenProtection");
                keys.Add("2.16.840.1.101.2.1.1.9", "INFOSEC_sdnsKeyManagement");
                #region 2.16.840.1.113730 NETSCAPE
                keys.Add("2.16.840.1.113730", "NETSCAPE");
                keys.Add("2.16.840.1.113730.1", "NETSCAPE_CERT_EXTENSION");
                keys.Add("2.16.840.1.113730.1.1", "NETSCAPE_CERT_TYPE");
                keys.Add("2.16.840.1.113730.1.12", "NETSCAPE_SSL_SERVER_NAME");
                keys.Add("2.16.840.1.113730.1.13", "NETSCAPE_COMMENT");
                keys.Add("2.16.840.1.113730.1.2", "NETSCAPE_BASE_URL");
                keys.Add("2.16.840.1.113730.1.3", "NETSCAPE_REVOCATION_URL");
                keys.Add("2.16.840.1.113730.1.4", "NETSCAPE_CA_REVOCATION_URL");
                keys.Add("2.16.840.1.113730.1.7", "NETSCAPE_CERT_RENEWAL_URL");
                keys.Add("2.16.840.1.113730.1.8", "NETSCAPE_CA_POLICY_URL");
                #endregion
                #endregion
                #region 2.5
                keys.Add("2.5", "DS");
                keys.Add("2.5.29.1", "AUTHORITY_KEY_IDENTIFIER");
                keys.Add("2.5.29.10", "BASIC_CONSTRAINTS");
                keys.Add("2.5.29.14", "SUBJECT_KEY_IDENTIFIER");
                keys.Add("2.5.29.15", "KEY_USAGE");
                keys.Add("2.5.29.16", "PRIVATEKEY_USAGE_PERIOD");
                keys.Add("2.5.29.17", "SUBJECT_ALT_NAME2");
                keys.Add("2.5.29.18", "ISSUER_ALT_NAME2");
                keys.Add("2.5.29.19", "BASIC_CONSTRAINTS2");
                keys.Add("2.5.29.2", "KEY_ATTRIBUTES");
                keys.Add("2.5.29.20", "CRL_NUMBER");
                keys.Add("2.5.29.21", "CRL_REASON_CODE");
                keys.Add("2.5.29.23", "REASON_CODE_HOLD");
                keys.Add("2.5.29.27", "DELTA_CRL_INDICATOR");
                keys.Add("2.5.29.28", "ISSUING_DIST_POINT");
                keys.Add("2.5.29.3", "CERT_POLICIES_95");
                keys.Add("2.5.29.30", "NAME_CONSTRAINTS");
                keys.Add("2.5.29.31", "CRL_DIST_POINTS");
                keys.Add("2.5.29.32", "CERT_POLICIES");
                keys.Add("2.5.29.32.0", "ANY_CERT_POLICY");
                keys.Add("2.5.29.33", "POLICY_MAPPINGS");
                keys.Add("2.5.29.35", "AUTHORITY_KEY_IDENTIFIER2");
                keys.Add("2.5.29.36", "POLICY_CONSTRAINTS");
                keys.Add("2.5.29.37", "ENHANCED_KEY_USAGE");
                keys.Add("2.5.29.4", "KEY_USAGE_RESTRICTION");
                keys.Add("2.5.29.46", "FRESHEST_CRL");
                keys.Add("2.5.29.5", "LEGACY_POLICY_MAPPINGS");
                keys.Add("2.5.29.7", "SUBJECT_ALT_NAME");
                keys.Add("2.5.29.8", "ISSUER_ALT_NAME");
                keys.Add("2.5.29.9", "SUBJECT_DIR_ATTRS");
                keys.Add("2.5.4.10", "ORGANIZATION_NAME");
                keys.Add("2.5.4.11", "ORGANIZATIONAL_UNIT_NAME");
                keys.Add("2.5.4.12", "TITLE");
                keys.Add("2.5.4.13", "DESCRIPTION");
                keys.Add("2.5.4.14", "SEARCH_GUIDE");
                keys.Add("2.5.4.15", "BUSINESS_CATEGORY");
                keys.Add("2.5.4.16", "POSTAL_ADDRESS");
                keys.Add("2.5.4.17", "POSTAL_CODE");
                keys.Add("2.5.4.18", "POST_OFFICE_BOX");
                keys.Add("2.5.4.19", "PHYSICAL_DELIVERY_OFFICE_NAME");
                keys.Add("2.5.4.20", "TELEPHONE_NUMBER");
                keys.Add("2.5.4.21", "TELEX_NUMBER");
                keys.Add("2.5.4.22", "TELETEXT_TERMINAL_IDENTIFIER");
                keys.Add("2.5.4.23", "FACSIMILE_TELEPHONE_NUMBER");
                keys.Add("2.5.4.24", "X21_ADDRESS");
                keys.Add("2.5.4.25", "INTERNATIONAL_ISDN_NUMBER");
                keys.Add("2.5.4.26", "REGISTERED_ADDRESS");
                keys.Add("2.5.4.27", "DESTINATION_INDICATOR");
                keys.Add("2.5.4.28", "PREFERRED_DELIVERY_METHOD");
                keys.Add("2.5.4.29", "PRESENTATION_ADDRESS");
                keys.Add("2.5.4.3", "COMMON_NAME");
                keys.Add("2.5.4.30", "SUPPORTED_APPLICATION_CONTEXT");
                keys.Add("2.5.4.31", "MEMBER");
                keys.Add("2.5.4.32", "OWNER");
                keys.Add("2.5.4.33", "ROLE_OCCUPANT");
                keys.Add("2.5.4.34", "SEE_ALSO");
                keys.Add("2.5.4.35", "USER_PASSWORD");
                keys.Add("2.5.4.36", "USER_CERTIFICATE");
                keys.Add("2.5.4.37", "CA_CERTIFICATE");
                keys.Add("2.5.4.38", "AUTHORITY_REVOCATION_LIST");
                keys.Add("2.5.4.39", "CERTIFICATE_REVOCATION_LIST");
                keys.Add("2.5.4.4", "SUR_NAME");
                keys.Add("2.5.4.40", "CROSS_CERTIFICATE_PAIR");
                keys.Add("2.5.4.42", "GIVEN_NAME");
                keys.Add("2.5.4.43", "INITIALS");
                keys.Add("2.5.4.46", "DN_QUALIFIER");
                keys.Add("2.5.4.5", "DEVICE_SERIAL_NUMBER");
                keys.Add("2.5.4.6", "COUNTRY_NAME");
                keys.Add("2.5.4.7", "LOCALITY_NAME");
                keys.Add("2.5.4.8", "STATE_OR_PROVINCE_NAME");
                keys.Add("2.5.4.9", "STREET_ADDRESS");
                keys.Add("2.5.8", "DSALG");
                keys.Add("2.5.8.1", "DSALG_CRPT");
                keys.Add("2.5.8.1.1", "DSALG_RSA");
                keys.Add("2.5.8.2", "DSALG_HASH");
                keys.Add("2.5.8.3", "DSALG_SIGN");
                #endregion
            }
            catch { }
        }
        public static string FindKey(string key)
        {
            return SOID_KEYS.keys[key];
        }
    }
    public class SOID
    {
        public const string szOID_RSA = "1.2.840.113549";
        public const string szOID_PKCS = "1.2.840.113549.1";
        public const string szOID_RSA_HASH = "1.2.840.113549.2";
        public const string szOID_RSA_ENCRYPT = "1.2.840.113549.3";
        public const string szOID_PKCS_1 = "1.2.840.113549.1.1";
        public const string szOID_PKCS_2 = "1.2.840.113549.1.2";
        public const string szOID_PKCS_3 = "1.2.840.113549.1.3";
        public const string szOID_PKCS_4 = "1.2.840.113549.1.4";
        public const string szOID_PKCS_5 = "1.2.840.113549.1.5";
        public const string szOID_PKCS_6 = "1.2.840.113549.1.6";
        public const string szOID_PKCS_7 = "1.2.840.113549.1.7";
        public const string szOID_PKCS_8 = "1.2.840.113549.1.8";
        public const string szOID_PKCS_9 = "1.2.840.113549.1.9";
        public const string szOID_PKCS_10 = "1.2.840.113549.1.10";
        public const string szOID_PKCS_11 = "1.2.840.113549.1.12";
        public const string szOID_RSA_RSA = "1.2.840.113549.1.1.1";
        public const string szOID_RSA_MD2RSA = "1.2.840.113549.1.1.2";
        public const string szOID_RSA_MD4RSA = "1.2.840.113549.1.1.3";
        public const string szOID_RSA_MD5RSA = "1.2.840.113549.1.1.4";
        public const string szOID_RSA_SHA1RSA = "1.2.840.113549.1.1.5";
        public const string szOID_RSA_SET0AEP_RSA = "1.2.840.113549.1.1.6";
        public const string szOID_RSA_DH = "1.2.840.113549.1.3.1";
        public const string szOID_RSA_data = "1.2.840.113549.1.7.1";
        public const string szOID_RSA_signedData = "1.2.840.113549.1.7.2";
        public const string szOID_RSA_envelopedData = "1.2.840.113549.1.7.3";
        public const string szOID_RSA_signEnvData = "1.2.840.113549.1.7.4";
        public const string szOID_RSA_digestedData = "1.2.840.113549.1.7.5";
        public const string szOID_RSA_hashedData = "1.2.840.113549.1.7.5";
        public const string szOID_RSA_encryptedData = "1.2.840.113549.1.7.6";
        public const string szOID_RSA_emailAddr = "1.2.840.113549.1.9.1";
        public const string szOID_RSA_unstructName = "1.2.840.113549.1.9.2";
        public const string szOID_RSA_contentType = "1.2.840.113549.1.9.3";
        public const string szOID_RSA_messageDigest = "1.2.840.113549.1.9.4";
        public const string szOID_RSA_signingTime = "1.2.840.113549.1.9.5";
        public const string szOID_RSA_counterSign = "1.2.840.113549.1.9.6";
        public const string szOID_RSA_challengePwd = "1.2.840.113549.1.9.7";
        public const string szOID_RSA_unstructAddr = "1.2.840.113549.1.9.9";
        public const string szOID_RSA_extCertAttrs = "1.2.840.113549.1.9.9";
        public const string szOID_RSA_certExtensions = "1.2.840.113549.1.9.14";
        public const string szOID_RSA_SMIMECapabilities = "1.2.840.113549.1.9.15";
        public const string szOID_RSA_preferSignedData = "1.2.840.113549.1.9.15.1";
        public const string szOID_RSA_SMIMEalg = "1.2.840.113549.1.9.16.3";
        public const string szOID_RSA_SMIMEalgESDH = "1.2.840.113549.1.9.16.3.5";
        public const string szOID_RSA_SMIMEalgCMS3DESwrap = "1.2.840.113549.1.9.16.3.6";
        public const string szOID_RSA_SMIMEalgCMSRC2wrap = "1.2.840.113549.1.9.16.3.7";
        public const string szOID_RSA_MD2 = "1.2.840.113549.2.2";
        public const string szOID_RSA_MD4 = "1.2.840.113549.2.4";
        public const string szOID_RSA_MD5 = "1.2.840.113549.2.5";
        public const string szOID_RSA_RC2CBC = "1.2.840.113549.3.2";
        public const string szOID_RSA_RC4 = "1.2.840.113549.3.4";
        public const string szOID_RSA_DES_EDE3_CBC = "1.2.840.113549.3.7";
        public const string szOID_RSA_RC5_CBCPad = "1.2.840.113549.3.9";
        public const string szOID_ANSI_X942 = "1.2.840.10046";
        public const string szOID_ANSI_X942_DH = "1.2.840.10046.2.1";
        public const string szOID_X957 = "1.2.840.10040";
        public const string szOID_X957_DSA = "1.2.840.10040.4.1";
        public const string szOID_X957_SHA1DSA = "1.2.840.10040.4.3";
        public const string szOID_DS = "2.5";
        public const string szOID_DSALG = "2.5.8";
        public const string szOID_DSALG_CRPT = "2.5.8.1";
        public const string szOID_DSALG_HASH = "2.5.8.2";
        public const string szOID_DSALG_SIGN = "2.5.8.3";
        public const string szOID_DSALG_RSA = "2.5.8.1.1";
        public const string szOID_OIW = "1.3.14";
        public const string szOID_OIWSEC = "1.3.14.3.2";
        public const string szOID_OIWSEC_md4RSA = "1.3.14.3.2.2";
        public const string szOID_OIWSEC_md5RSA = "1.3.14.3.2.3";
        public const string szOID_OIWSEC_md4RSA2 = "1.3.14.3.2.4";
        public const string szOID_OIWSEC_desECB = "1.3.14.3.2.6";
        public const string szOID_OIWSEC_desCBC = "1.3.14.3.2.7";
        public const string szOID_OIWSEC_desOFB = "1.3.14.3.2.8";
        public const string szOID_OIWSEC_desCFB = "1.3.14.3.2.9";
        public const string szOID_OIWSEC_desMAC = "1.3.14.3.2.10";
        public const string szOID_OIWSEC_rsaSign = "1.3.14.3.2.11";
        public const string szOID_OIWSEC_dsa = "1.3.14.3.2.12";
        public const string szOID_OIWSEC_shaDSA = "1.3.14.3.2.13";
        public const string szOID_OIWSEC_mdc2RSA = "1.3.14.3.2.14";
        public const string szOID_OIWSEC_shaRSA = "1.3.14.3.2.15";
        public const string szOID_OIWSEC_dhCommMod = "1.3.14.3.2.16";
        public const string szOID_OIWSEC_desEDE = "1.3.14.3.2.17";
        public const string szOID_OIWSEC_sha = "1.3.14.3.2.18";
        public const string szOID_OIWSEC_mdc2 = "1.3.14.3.2.19";
        public const string szOID_OIWSEC_dsaComm = "1.3.14.3.2.20";
        public const string szOID_OIWSEC_dsaCommSHA = "1.3.14.3.2.21";
        public const string szOID_OIWSEC_rsaXchg = "1.3.14.3.2.22";
        public const string szOID_OIWSEC_keyHashSeal = "1.3.14.3.2.23";
        public const string szOID_OIWSEC_md2RSASign = "1.3.14.3.2.24";
        public const string szOID_OIWSEC_md5RSASign = "1.3.14.3.2.25";
        public const string szOID_OIWSEC_sha1 = "1.3.14.3.2.26";
        public const string szOID_OIWSEC_dsaSHA1 = "1.3.14.3.2.27";
        public const string szOID_OIWSEC_dsaCommSHA1 = "1.3.14.3.2.28";
        public const string szOID_OIWSEC_sha1RSASign = "1.3.14.3.2.29";
        public const string szOID_OIWDIR = "1.3.14.7.2";
        public const string szOID_OIWDIR_CRPT = "1.3.14.7.2.1";
        public const string szOID_OIWDIR_HASH = "1.3.14.7.2.2";
        public const string szOID_OIWDIR_SIGN = "1.3.14.7.2.3";
        public const string szOID_OIWDIR_md2 = "1.3.14.7.2.2.1";
        public const string szOID_OIWDIR_md2RSA = "1.3.14.7.2.3.1";
        public const string szOID_INFOSEC = "2.16.840.1.101.2.1";
        public const string szOID_INFOSEC_sdnsSignature = "2.16.840.1.101.2.1.1.1";
        public const string szOID_INFOSEC_mosaicSignature = "2.16.840.1.101.2.1.1.2";
        public const string szOID_INFOSEC_sdnsConfidentiality = "2.16.840.1.101.2.1.1.3";
        public const string szOID_INFOSEC_mosaicConfidentiality = "2.16.840.1.101.2.1.1.4";
        public const string szOID_INFOSEC_sdnsIntegrity = "2.16.840.1.101.2.1.1.5";
        public const string szOID_INFOSEC_mosaicIntegrity = "2.16.840.1.101.2.1.1.6";
        public const string szOID_INFOSEC_sdnsTokenProtection = "2.16.840.1.101.2.1.1.7";
        public const string szOID_INFOSEC_mosaicTokenProtection = "2.16.840.1.101.2.1.1.8";
        public const string szOID_INFOSEC_sdnsKeyManagement = "2.16.840.1.101.2.1.1.9";
        public const string szOID_INFOSEC_mosaicKeyManagement = "2.16.840.1.101.2.1.1.10";
        public const string szOID_INFOSEC_sdnsKMandSig = "2.16.840.1.101.2.1.1.11";
        public const string szOID_INFOSEC_mosaicKMandSig = "2.16.840.1.101.2.1.1.12";
        public const string szOID_INFOSEC_SuiteASignature = "2.16.840.1.101.2.1.1.13";
        public const string szOID_INFOSEC_SuiteAConfidentiality = "2.16.840.1.101.2.1.1.14";
        public const string szOID_INFOSEC_SuiteAIntegrity = "2.16.840.1.101.2.1.1.15";
        public const string szOID_INFOSEC_SuiteATokenProtection = "2.16.840.1.101.2.1.1.16";
        public const string szOID_INFOSEC_SuiteAKeyManagement = "2.16.840.1.101.2.1.1.17";
        public const string szOID_INFOSEC_SuiteAKMandSig = "2.16.840.1.101.2.1.1.18";
        public const string szOID_INFOSEC_mosaicUpdatedSig = "2.16.840.1.101.2.1.1.19";
        public const string szOID_INFOSEC_mosaicKMandUpdSig = "2.16.840.1.101.2.1.1.20";
        public const string szOID_INFOSEC_mosaicUpdateInteg = "2.16.840.1.101.2.1.1.21";
        public const string szOID_COMMON_NAME = "2.5.4.3";
        public const string szOID_SUR_NAME = "2.5.4.4";
        public const string szOID_DEVICE_SERIAL_NUMBER = "2.5.4.5";
        public const string szOID_COUNTRY_NAME = "2.5.4.6";
        public const string szOID_LOCALITY_NAME = "2.5.4.7";
        public const string szOID_STATE_OR_PROVINCE_NAME = "2.5.4.8";
        public const string szOID_STREET_ADDRESS = "2.5.4.9";
        public const string szOID_ORGANIZATION_NAME = "2.5.4.10";
        public const string szOID_ORGANIZATIONAL_UNIT_NAME = "2.5.4.11";
        public const string szOID_TITLE = "2.5.4.12";
        public const string szOID_DESCRIPTION = "2.5.4.13";
        public const string szOID_SEARCH_GUIDE = "2.5.4.14";
        public const string szOID_BUSINESS_CATEGORY = "2.5.4.15";
        public const string szOID_POSTAL_ADDRESS = "2.5.4.16";
        public const string szOID_POSTAL_CODE = "2.5.4.17";
        public const string szOID_POST_OFFICE_BOX = "2.5.4.18";
        public const string szOID_PHYSICAL_DELIVERY_OFFICE_NAME = "2.5.4.19";
        public const string szOID_TELEPHONE_NUMBER = "2.5.4.20";
        public const string szOID_TELEX_NUMBER = "2.5.4.21";
        public const string szOID_TELETEXT_TERMINAL_IDENTIFIER = "2.5.4.22";
        public const string szOID_FACSIMILE_TELEPHONE_NUMBER = "2.5.4.23";
        public const string szOID_X21_ADDRESS = "2.5.4.24";
        public const string szOID_INTERNATIONAL_ISDN_NUMBER = "2.5.4.25";
        public const string szOID_REGISTERED_ADDRESS = "2.5.4.26";
        public const string szOID_DESTINATION_INDICATOR = "2.5.4.27";
        public const string szOID_PREFERRED_DELIVERY_METHOD = "2.5.4.28";
        public const string szOID_PRESENTATION_ADDRESS = "2.5.4.29";
        public const string szOID_SUPPORTED_APPLICATION_CONTEXT = "2.5.4.30";
        public const string szOID_MEMBER = "2.5.4.31";
        public const string szOID_OWNER = "2.5.4.32";
        public const string szOID_ROLE_OCCUPANT = "2.5.4.33";
        public const string szOID_SEE_ALSO = "2.5.4.34";
        public const string szOID_USER_PASSWORD = "2.5.4.35";
        public const string szOID_USER_CERTIFICATE = "2.5.4.36";
        public const string szOID_CA_CERTIFICATE = "2.5.4.37";
        public const string szOID_AUTHORITY_REVOCATION_LIST = "2.5.4.38";
        public const string szOID_CERTIFICATE_REVOCATION_LIST = "2.5.4.39";
        public const string szOID_CROSS_CERTIFICATE_PAIR = "2.5.4.40";
        public const string szOID_GIVEN_NAME = "2.5.4.42";
        public const string szOID_INITIALS = "2.5.4.43";
        public const string szOID_DN_QUALIFIER = "2.5.4.46";
        public const string szOID_AUTHORITY_KEY_IDENTIFIER = "2.5.29.1";
        public const string szOID_KEY_ATTRIBUTES = "2.5.29.2";
        public const string szOID_CERT_POLICIES_95 = "2.5.29.3";
        public const string szOID_KEY_USAGE_RESTRICTION = "2.5.29.4";
        public const string szOID_LEGACY_POLICY_MAPPINGS = "2.5.29.5";
        public const string szOID_SUBJECT_ALT_NAME = "2.5.29.7";
        public const string szOID_ISSUER_ALT_NAME = "2.5.29.8";
        public const string szOID_SUBJECT_DIR_ATTRS = "2.5.29.9";
        public const string szOID_BASIC_CONSTRAINTS = "2.5.29.10";
        public const string szOID_SUBJECT_KEY_IDENTIFIER = "2.5.29.14";
        public const string szOID_KEY_USAGE = "2.5.29.15";
        public const string szOID_PRIVATEKEY_USAGE_PERIOD = "2.5.29.16";
        public const string szOID_SUBJECT_ALT_NAME2 = "2.5.29.17";
        public const string szOID_ISSUER_ALT_NAME2 = "2.5.29.18";
        public const string szOID_BASIC_CONSTRAINTS2 = "2.5.29.19";
        public const string szOID_CRL_NUMBER = "2.5.29.20";
        public const string szOID_CRL_REASON_CODE = "2.5.29.21";
        public const string szOID_REASON_CODE_HOLD = "2.5.29.23";
        public const string szOID_DELTA_CRL_INDICATOR = "2.5.29.27";
        public const string szOID_ISSUING_DIST_POINT = "2.5.29.28";
        public const string szOID_NAME_CONSTRAINTS = "2.5.29.30";
        public const string szOID_CRL_DIST_POINTS = "2.5.29.31";
        public const string szOID_CERT_POLICIES = "2.5.29.32";
        public const string szOID_ANY_CERT_POLICY = "2.5.29.32.0";
        public const string szOID_POLICY_MAPPINGS = "2.5.29.33";
        public const string szOID_AUTHORITY_KEY_IDENTIFIER2 = "2.5.29.35";
        public const string szOID_POLICY_CONSTRAINTS = "2.5.29.36";
        public const string szOID_ENHANCED_KEY_USAGE = "2.5.29.37";
        public const string szOID_FRESHEST_CRL = "2.5.29.46";
        public const string szOID_DOMAIN_COMPONENT = "0.9.2342.19200300.100.1.25";
        public const string szOID_PKCS_12_FRIENDLY_NAME_ATTR = "1.2.840.113549.1.9.20";
        public const string szOID_PKCS_12_LOCAL_KEY_ID = "1.2.840.113549.1.9.21";
        public const string szOID_CERT_EXTENSIONS = "1.3.6.1.4.1.311.2.1.14";
        public const string szOID_NEXT_UPDATE_LOCATION = "1.3.6.1.4.1.311.10.2";
        public const string szOID_KP_CTL_USAGE_SIGNING = "1.3.6.1.4.1.311.10.3.1";
        public const string szOID_KP_TIME_STAMP_SIGNING = "1.3.6.1.4.1.311.10.3.2";
        public const string szOID_KP_EFS = "1.3.6.1.4.1.311.10.3.4";
        public const string szOID_EFS_RECOVERY = "1.3.6.1.4.1.311.10.3.4.1";
        public const string szOID_WHQL_CRYPTO = "1.3.6.1.4.1.311.10.3.5";
        public const string szOID_NT5_CRYPTO = "1.3.6.1.4.1.311.10.3.6";
        public const string szOID_OEM_WHQL_CRYPTO = "1.3.6.1.4.1.311.10.3.7";
        public const string szOID_EMBEDDED_NT_CRYPTO = "1.3.6.1.4.1.311.10.3.8";
        public const string szOID_ROOT_LIST_SIGNER = "1.3.6.1.4.1.311.10.3.9";
        public const string szOID_KP_QUALIFIED_SUBORDINATION = "1.3.6.1.4.1.311.10.3.10";
        public const string szOID_KP_KEY_RECOVERY = "1.3.6.1.4.1.311.10.3.11";
        public const string szOID_KP_DOCUMENT_SIGNING = "1.3.6.1.4.1.311.10.3.12";
        public const string szOID_KP_LIFETIME_SIGNING = "1.3.6.1.4.1.311.10.3.13";
        public const string szOID_KP_MOBILE_DEVICE_SOFTWARE = "1.3.6.1.4.1.311.10.3.14";
        public const string szOID_YESNO_TRUST_ATTR = "1.3.6.1.4.1.311.10.4.1";
        public const string szOID_REMOVE_CERTIFICATE = "1.3.6.1.4.1.311.10.8.1";
        public const string szOID_CROSS_CERT_DIST_POINTS = "1.3.6.1.4.1.311.10.9.1";
        public const string szOID_CTL = "1.3.6.1.4.1.311.10.10.1";
        public const string szOID_SORTED_CTL = "1.3.6.1.4.1.311.10.10.1.1";
        public const string szOID_ANY_APPLICATION_POLICY = "1.3.6.1.4.1.311.10.12.1";
        public const string szOID_RENEWAL_CERTIFICATE = "1.3.6.1.4.1.311.13.1";
        public const string szOID_ENROLLMENT_NAME_VALUE_PAIR = "1.3.6.1.4.1.311.13.2.1";
        public const string szOID_ENROLLMENT_CSP_PROVIDER = "1.3.6.1.4.1.311.13.2.2";
        public const string szOID_OS_VERSION = "1.3.6.1.4.1.311.13.2.3";
        public const string szOID_PKCS_12_KEY_PROVIDER_NAME_ATTR = "1.3.6.1.4.1.311.17.1";
        public const string szOID_LOCAL_MACHINE_KEYSET = "1.3.6.1.4.1.311.17.2";
        public const string szOID_AUTO_ENROLL_CTL_USAGE = "1.3.6.1.4.1.311.20.1";
        public const string szOID_ENROLL_CERTTYPE_EXTENSION = "1.3.6.1.4.1.311.20.2";
        public const string szOID_ENROLLMENT_AGENT = "1.3.6.1.4.1.311.20.2.1";
        public const string szOID_KP_SMARTCARD_LOGON = "1.3.6.1.4.1.311.20.2.2";
        public const string szOID_CERT_MANIFOLD = "1.3.6.1.4.1.311.20.3";
        public const string szOID_CERTSRV_PREVIOUS_CERT_HASH = "1.3.6.1.4.1.311.21.2";
        public const string szOID_CRL_VIRTUAL_BASE = "1.3.6.1.4.1.311.21.3";
        public const string szOID_CRL_NEXT_PUBLISH = "1.3.6.1.4.1.311.21.4";
        public const string szOID_KP_CA_EXCHANGE = "1.3.6.1.4.1.311.21.5";
        public const string szOID_KP_KEY_RECOVERY_AGENT = "1.3.6.1.4.1.311.21.6";
        public const string szOID_CERTIFICATE_TEMPLATE = "1.3.6.1.4.1.311.21.7";
        public const string szOID_ENTERPRISE_OID_ROOT = "1.3.6.1.4.1.311.21.8";
        public const string szOID_RDN_DUMMY_SIGNER = "1.3.6.1.4.1.311.21.9";
        public const string szOID_APPLICATION_CERT_POLICIES = "1.3.6.1.4.1.311.21.10";
        public const string szOID_APPLICATION_POLICY_MAPPINGS = "1.3.6.1.4.1.311.21.11";
        public const string szOID_APPLICATION_POLICY_CONSTRAINTS = "1.3.6.1.4.1.311.21.12";
        public const string szOID_ARCHIVED_KEY_ATTR = "1.3.6.1.4.1.311.21.13";
        public const string szOID_CRL_SELF_CDP = "1.3.6.1.4.1.311.21.14";
        public const string szOID_REQUIRE_CERT_CHAIN_POLICY = "1.3.6.1.4.1.311.21.15";
        public const string szOID_ARCHIVED_KEY_CERT_HASH = "1.3.6.1.4.1.311.21.16";
        public const string szOID_ISSUED_CERT_HASH = "1.3.6.1.4.1.311.21.17";
        public const string szOID_DS_EMAIL_REPLICATION = "1.3.6.1.4.1.311.21.19";
        public const string szOID_REQUEST_CLIENT_INFO = "1.3.6.1.4.1.311.21.20";
        public const string szOID_ENCRYPTED_KEY_HASH = "1.3.6.1.4.1.311.21.21";
        public const string szOID_CERTSRV_CROSSCA_VERSION = "1.3.6.1.4.1.311.21.22";
        public const string szOID_KEYID_RDN = "1.3.6.1.4.1.311.10.7.1";
        public const string szOID_PKIX = "1.3.6.1.5.5.7";
        public const string szOID_PKIX_PE = "1.3.6.1.5.5.7.1";
        public const string szOID_AUTHORITY_INFO_ACCESS = "1.3.6.1.5.5.7.1.1";
        public const string szOID_PKIX_POLICY_QUALIFIER_CPS = "1.3.6.1.5.5.7.2.1";
        public const string szOID_PKIX_POLICY_QUALIFIER_USERNOTICE = "1.3.6.1.5.5.7.2.2";
        public const string szOID_PKIX_KP = "1.3.6.1.5.5.7.3";
        public const string szOID_PKIX_KP_SERVER_AUTH = "1.3.6.1.5.5.7.3.1";
        public const string szOID_PKIX_KP_CLIENT_AUTH = "1.3.6.1.5.5.7.3.2";
        public const string szOID_PKIX_KP_CODE_SIGNING = "1.3.6.1.5.5.7.3.3";
        public const string szOID_PKIX_KP_EMAIL_PROTECTION = "1.3.6.1.5.5.7.3.4";
        public const string szOID_PKIX_KP_IPSEC_END_SYSTEM = "1.3.6.1.5.5.7.3.5";
        public const string szOID_PKIX_KP_IPSEC_TUNNEL = "1.3.6.1.5.5.7.3.6";
        public const string szOID_PKIX_KP_IPSEC_USER = "1.3.6.1.5.5.7.3.7";
        public const string szOID_PKIX_KP_TIMESTAMP_SIGNING = "1.3.6.1.5.5.7.3.8";
        public const string szOID_PKIX_NO_SIGNATURE = "1.3.6.1.5.5.7.6.2";
        public const string szOID_CMC = "1.3.6.1.5.5.7.7";
        public const string szOID_CMC_STATUS_INFO = "1.3.6.1.5.5.7.7.1";
        public const string szOID_CMC_IDENTIFICATION = "1.3.6.1.5.5.7.7.2";
        public const string szOID_CMC_IDENTITY_PROOF = "1.3.6.1.5.5.7.7.3";
        public const string szOID_CMC_DATA_RETURN = "1.3.6.1.5.5.7.7.4";
        public const string szOID_CMC_TRANSACTION_ID = "1.3.6.1.5.5.7.7.5";
        public const string szOID_CMC_SENDER_NONCE = "1.3.6.1.5.5.7.7.6";
        public const string szOID_CMC_RECIPIENT_NONCE = "1.3.6.1.5.5.7.7.7";
        public const string szOID_CMC_ADD_EXTENSIONS = "1.3.6.1.5.5.7.7.8";
        public const string szOID_CMC_ENCRYPTED_POP = "1.3.6.1.5.5.7.7.9";
        public const string szOID_CMC_DECRYPTED_POP = "1.3.6.1.5.5.7.7.10";
        public const string szOID_CMC_LRA_POP_WITNESS = "1.3.6.1.5.5.7.7.11";
        public const string szOID_CMC_GET_CERT = "1.3.6.1.5.5.7.7.15";
        public const string szOID_CMC_GET_CRL = "1.3.6.1.5.5.7.7.16";
        public const string szOID_CMC_REVOKE_REQUEST = "1.3.6.1.5.5.7.7.17";
        public const string szOID_CMC_REG_INFO = "1.3.6.1.5.5.7.7.18";
        public const string szOID_CMC_RESPONSE_INFO = "1.3.6.1.5.5.7.7.19";
        public const string szOID_CMC_QUERY_PENDING = "1.3.6.1.5.5.7.7.21";
        public const string szOID_CMC_ID_POP_LINK_RANDOM = "1.3.6.1.5.5.7.7.22";
        public const string szOID_CMC_ID_POP_LINK_WITNESS = "1.3.6.1.5.5.7.7.23";
        public const string szOID_CT_PKI_DATA = "1.3.6.1.5.5.7.12.2";
        public const string szOID_CT_PKI_RESPONSE = "1.3.6.1.5.5.7.12.3";
        public const string szOID_PKIX_ACC_DESCR = "1.3.6.1.5.5.7.48";
        public const string szOID_PKIX_OCSP = "1.3.6.1.5.5.7.48.1";
        public const string szOID_PKIX_CA_ISSUERS = "1.3.6.1.5.5.7.48.2";
        public const string szOID_IPSEC_KP_IKE_INTERMEDIATE = "1.3.6.1.5.5.8.2.2";
        public const string szOID_NETSCAPE = "2.16.840.1.113730";
        public const string szOID_NETSCAPE_CERT_EXTENSION = "2.16.840.1.113730.1";
        public const string szOID_NETSCAPE_CERT_TYPE = "2.16.840.1.113730.1.1";
        public const string szOID_NETSCAPE_BASE_URL = "2.16.840.1.113730.1.2";
        public const string szOID_NETSCAPE_REVOCATION_URL = "2.16.840.1.113730.1.3";
        public const string szOID_NETSCAPE_CA_REVOCATION_URL = "2.16.840.1.113730.1.4";
        public const string szOID_NETSCAPE_CERT_RENEWAL_URL = "2.16.840.1.113730.1.7";
        public const string szOID_NETSCAPE_CA_POLICY_URL = "2.16.840.1.113730.1.8";
        public const string szOID_NETSCAPE_SSL_SERVER_NAME = "2.16.840.1.113730.1.12";
        public const string szOID_NETSCAPE_COMMENT = "2.16.840.1.113730.1.13";
    }
    /*  public enum SOI_TAG
      {
          DOMAIN_COMPONENT = "0.9.2342.19200300.100.1.25",
          X957 = "1.2.840.10040",
          X957_DSA = "1.2.840.10040.4.1",
          X957_SHA1DSA = "1.2.840.10040.4.3",
          ANSI_X942 = "1.2.840.10046",
          ANSI_X942_DH = "1.2.840.10046.2.1",
          RSA = "1.2.840.113549",
          PKCS = "1.2.840.113549.1",
          PKCS_1 = "1.2.840.113549.1.1",
          RSA_RSA = "1.2.840.113549.1.1.1",
          RSA_MD2RSA = "1.2.840.113549.1.1.2",
          RSA_MD4RSA = "1.2.840.113549.1.1.3",
          RSA_MD5RSA = "1.2.840.113549.1.1.4",
          RSA_SHA1RSA = "1.2.840.113549.1.1.5",
          RSA_SET0AEP_RSA = "1.2.840.113549.1.1.6",
          PKCS_10 = "1.2.840.113549.1.10",
          PKCS_11 = "1.2.840.113549.1.12",
          PKCS_2 = "1.2.840.113549.1.2",
          PKCS_3 = "1.2.840.113549.1.3",
          RSA_DH = "1.2.840.113549.1.3.1",
          PKCS_4 = "1.2.840.113549.1.4",
          PKCS_5 = "1.2.840.113549.1.5",
          PKCS_6 = "1.2.840.113549.1.6",
          PKCS_7 = "1.2.840.113549.1.7",
          RSA_data = "1.2.840.113549.1.7.1",
          RSA_signedData = "1.2.840.113549.1.7.2",
          RSA_envelopedData = "1.2.840.113549.1.7.3",
          RSA_signEnvData = "1.2.840.113549.1.7.4",
          RSA_digestedData = "1.2.840.113549.1.7.5",
          RSA_hashedData = "1.2.840.113549.1.7.5",
          RSA_encryptedData = "1.2.840.113549.1.7.6",
          PKCS_8 = "1.2.840.113549.1.8",
          PKCS_9 = "1.2.840.113549.1.9",
          RSA_emailAddr = "1.2.840.113549.1.9.1",
          RSA_certExtensions = "1.2.840.113549.1.9.14",
          RSA_SMIMECapabilities = "1.2.840.113549.1.9.15",
          RSA_preferSignedData = "1.2.840.113549.1.9.15.1",
          RSA_SMIMEalg = "1.2.840.113549.1.9.16.3",
          RSA_SMIMEalgESDH = "1.2.840.113549.1.9.16.3.5",
          RSA_SMIMEalgCMS3DESwrap = "1.2.840.113549.1.9.16.3.6",
          RSA_SMIMEalgCMSRC2wrap = "1.2.840.113549.1.9.16.3.7",
          RSA_unstructName = "1.2.840.113549.1.9.2",
          PKCS_12_FRIENDLY_NAME_ATTR = "1.2.840.113549.1.9.20",
          PKCS_12_LOCAL_KEY_ID = "1.2.840.113549.1.9.21",
          RSA_contentType = "1.2.840.113549.1.9.3",
          RSA_messageDigest = "1.2.840.113549.1.9.4",
          RSA_signingTime = "1.2.840.113549.1.9.5",
          RSA_counterSign = "1.2.840.113549.1.9.6",
          RSA_challengePwd = "1.2.840.113549.1.9.7",
          RSA_unstructAddr = "1.2.840.113549.1.9.9",
          RSA_extCertAttrs = "1.2.840.113549.1.9.9",
          RSA_HASH = "1.2.840.113549.2",
          RSA_MD2 = "1.2.840.113549.2.2",
          RSA_MD4 = "1.2.840.113549.2.4",
          RSA_MD5 = "1.2.840.113549.2.5",
          RSA_ENCRYPT = "1.2.840.113549.3",
          RSA_RC2CBC = "1.2.840.113549.3.2",
          RSA_RC4 = "1.2.840.113549.3.4",
          RSA_DES_EDE3_CBC = "1.2.840.113549.3.7",
          RSA_RC5_CBCPad = "1.2.840.113549.3.9",
          OIW = "1.3.14",
          OIWSEC = "1.3.14.3.2",
          OIWSEC_desMAC = "1.3.14.3.2.10",
          OIWSEC_rsaSign = "1.3.14.3.2.11",
          OIWSEC_dsa = "1.3.14.3.2.12",
          OIWSEC_shaDSA = "1.3.14.3.2.13",
          OIWSEC_mdc2RSA = "1.3.14.3.2.14",
          OIWSEC_shaRSA = "1.3.14.3.2.15",
          OIWSEC_dhCommMod = "1.3.14.3.2.16",
          OIWSEC_desEDE = "1.3.14.3.2.17",
          OIWSEC_sha = "1.3.14.3.2.18",
          OIWSEC_mdc2 = "1.3.14.3.2.19",
          OIWSEC_md4RSA = "1.3.14.3.2.2",
          OIWSEC_dsaComm = "1.3.14.3.2.20",
          OIWSEC_dsaCommSHA = "1.3.14.3.2.21",
          OIWSEC_rsaXchg = "1.3.14.3.2.22",
          OIWSEC_keyHashSeal = "1.3.14.3.2.23",
          OIWSEC_md2RSASign = "1.3.14.3.2.24",
          OIWSEC_md5RSASign = "1.3.14.3.2.25",
          OIWSEC_sha1 = "1.3.14.3.2.26",
          OIWSEC_dsaSHA1 = "1.3.14.3.2.27",
          OIWSEC_dsaCommSHA1 = "1.3.14.3.2.28",
          OIWSEC_sha1RSASign = "1.3.14.3.2.29",
          OIWSEC_md5RSA = "1.3.14.3.2.3",
          OIWSEC_md4RSA2 = "1.3.14.3.2.4",
          OIWSEC_desECB = "1.3.14.3.2.6",
          OIWSEC_desCBC = "1.3.14.3.2.7",
          OIWSEC_desOFB = "1.3.14.3.2.8",
          OIWSEC_desCFB = "1.3.14.3.2.9",
          OIWDIR = "1.3.14.7.2",
          OIWDIR_CRPT = "1.3.14.7.2.1",
          OIWDIR_HASH = "1.3.14.7.2.2",
          OIWDIR_md2 = "1.3.14.7.2.2.1",
          OIWDIR_SIGN = "1.3.14.7.2.3",
          OIWDIR_md2RSA = "1.3.14.7.2.3.1",
          CTL = "1.3.6.1.4.1.311.10.10.1",
          SORTED_CTL = "1.3.6.1.4.1.311.10.10.1.1",
          ANY_APPLICATION_POLICY = "1.3.6.1.4.1.311.10.12.1",
          NEXT_UPDATE_LOCATION = "1.3.6.1.4.1.311.10.2",
          KP_CTL_USAGE_SIGNING = "1.3.6.1.4.1.311.10.3.1",
          KP_QUALIFIED_SUBORDINATION = "1.3.6.1.4.1.311.10.3.10",
          KP_KEY_RECOVERY = "1.3.6.1.4.1.311.10.3.11",
          KP_DOCUMENT_SIGNING = "1.3.6.1.4.1.311.10.3.12",
          KP_LIFETIME_SIGNING = "1.3.6.1.4.1.311.10.3.13",
          KP_MOBILE_DEVICE_SOFTWARE = "1.3.6.1.4.1.311.10.3.14",
          KP_TIME_STAMP_SIGNING = "1.3.6.1.4.1.311.10.3.2",
          KP_EFS = "1.3.6.1.4.1.311.10.3.4",
          EFS_RECOVERY = "1.3.6.1.4.1.311.10.3.4.1",
          WHQL_CRYPTO = "1.3.6.1.4.1.311.10.3.5",
          NT5_CRYPTO = "1.3.6.1.4.1.311.10.3.6",
          OEM_WHQL_CRYPTO = "1.3.6.1.4.1.311.10.3.7",
          EMBEDDED_NT_CRYPTO = "1.3.6.1.4.1.311.10.3.8",
          ROOT_LIST_SIGNER = "1.3.6.1.4.1.311.10.3.9",
          YESNO_TRUST_ATTR = "1.3.6.1.4.1.311.10.4.1",
          KEYID_RDN = "1.3.6.1.4.1.311.10.7.1",
          REMOVE_CERTIFICATE = "1.3.6.1.4.1.311.10.8.1",
          CROSS_CERT_DIST_POINTS = "1.3.6.1.4.1.311.10.9.1",
          RENEWAL_CERTIFICATE = "1.3.6.1.4.1.311.13.1",
          ENROLLMENT_NAME_VALUE_PAIR = "1.3.6.1.4.1.311.13.2.1",
          ENROLLMENT_CSP_PROVIDER = "1.3.6.1.4.1.311.13.2.2",
          OS_VERSION = "1.3.6.1.4.1.311.13.2.3",
          PKCS_12_KEY_PROVIDER_NAME_ATTR = "1.3.6.1.4.1.311.17.1",
          LOCAL_MACHINE_KEYSET = "1.3.6.1.4.1.311.17.2",
          CERT_EXTENSIONS = "1.3.6.1.4.1.311.2.1.14",
          AUTO_ENROLL_CTL_USAGE = "1.3.6.1.4.1.311.20.1",
          ENROLL_CERTTYPE_EXTENSION = "1.3.6.1.4.1.311.20.2",
          ENROLLMENT_AGENT = "1.3.6.1.4.1.311.20.2.1",
          KP_SMARTCARD_LOGON = "1.3.6.1.4.1.311.20.2.2",
          CERT_MANIFOLD = "1.3.6.1.4.1.311.20.3",
          APPLICATION_CERT_POLICIES = "1.3.6.1.4.1.311.21.10",
          APPLICATION_POLICY_MAPPINGS = "1.3.6.1.4.1.311.21.11",
          APPLICATION_POLICY_CONSTRAINTS = "1.3.6.1.4.1.311.21.12",
          ARCHIVED_KEY_ATTR = "1.3.6.1.4.1.311.21.13",
          CRL_SELF_CDP = "1.3.6.1.4.1.311.21.14",
          REQUIRE_CERT_CHAIN_POLICY = "1.3.6.1.4.1.311.21.15",
          ARCHIVED_KEY_CERT_HASH = "1.3.6.1.4.1.311.21.16",
          ISSUED_CERT_HASH = "1.3.6.1.4.1.311.21.17",
          DS_EMAIL_REPLICATION = "1.3.6.1.4.1.311.21.19",
          CERTSRV_PREVIOUS_CERT_HASH = "1.3.6.1.4.1.311.21.2",
          REQUEST_CLIENT_INFO = "1.3.6.1.4.1.311.21.20",
          ENCRYPTED_KEY_HASH = "1.3.6.1.4.1.311.21.21",
          CERTSRV_CROSSCA_VERSION = "1.3.6.1.4.1.311.21.22",
          CRL_VIRTUAL_BASE = "1.3.6.1.4.1.311.21.3",
          CRL_NEXT_PUBLISH = "1.3.6.1.4.1.311.21.4",
          KP_CA_EXCHANGE = "1.3.6.1.4.1.311.21.5",
          KP_KEY_RECOVERY_AGENT = "1.3.6.1.4.1.311.21.6",
          CERTIFICATE_TEMPLATE = "1.3.6.1.4.1.311.21.7",
          ENTERPRISE_OID_ROOT = "1.3.6.1.4.1.311.21.8",
          RDN_DUMMY_SIGNER = "1.3.6.1.4.1.311.21.9",
          PKIX = "1.3.6.1.5.5.7",
          PKIX_PE = "1.3.6.1.5.5.7.1",
          AUTHORITY_INFO_ACCESS = "1.3.6.1.5.5.7.1.1",
          CT_PKI_DATA = "1.3.6.1.5.5.7.12.2",
          CT_PKI_RESPONSE = "1.3.6.1.5.5.7.12.3",
          PKIX_POLICY_QUALIFIER_CPS = "1.3.6.1.5.5.7.2.1",
          PKIX_POLICY_QUALIFIER_USERNOTICE = "1.3.6.1.5.5.7.2.2",
          PKIX_KP = "1.3.6.1.5.5.7.3",
          PKIX_KP_SERVER_AUTH = "1.3.6.1.5.5.7.3.1",
          PKIX_KP_CLIENT_AUTH = "1.3.6.1.5.5.7.3.2",
          PKIX_KP_CODE_SIGNING = "1.3.6.1.5.5.7.3.3",
          PKIX_KP_EMAIL_PROTECTION = "1.3.6.1.5.5.7.3.4",
          PKIX_KP_IPSEC_END_SYSTEM = "1.3.6.1.5.5.7.3.5",
          PKIX_KP_IPSEC_TUNNEL = "1.3.6.1.5.5.7.3.6",
          PKIX_KP_IPSEC_USER = "1.3.6.1.5.5.7.3.7",
          PKIX_KP_TIMESTAMP_SIGNING = "1.3.6.1.5.5.7.3.8",
          PKIX_ACC_DESCR = "1.3.6.1.5.5.7.48",
          PKIX_OCSP = "1.3.6.1.5.5.7.48.1",
          PKIX_CA_ISSUERS = "1.3.6.1.5.5.7.48.2",
          PKIX_NO_SIGNATURE = "1.3.6.1.5.5.7.6.2",
          CMC = "1.3.6.1.5.5.7.7",
          CMC_STATUS_INFO = "1.3.6.1.5.5.7.7.1",
          CMC_DECRYPTED_POP = "1.3.6.1.5.5.7.7.10",
          CMC_LRA_POP_WITNESS = "1.3.6.1.5.5.7.7.11",
          CMC_GET_CERT = "1.3.6.1.5.5.7.7.15",
          CMC_GET_CRL = "1.3.6.1.5.5.7.7.16",
          CMC_REVOKE_REQUEST = "1.3.6.1.5.5.7.7.17",
          CMC_REG_INFO = "1.3.6.1.5.5.7.7.18",
          CMC_RESPONSE_INFO = "1.3.6.1.5.5.7.7.19",
          CMC_IDENTIFICATION = "1.3.6.1.5.5.7.7.2",
          CMC_QUERY_PENDING = "1.3.6.1.5.5.7.7.21",
          CMC_ID_POP_LINK_RANDOM = "1.3.6.1.5.5.7.7.22",
          CMC_ID_POP_LINK_WITNESS = "1.3.6.1.5.5.7.7.23",
          CMC_IDENTITY_PROOF = "1.3.6.1.5.5.7.7.3",
          CMC_DATA_RETURN = "1.3.6.1.5.5.7.7.4",
          CMC_TRANSACTION_ID = "1.3.6.1.5.5.7.7.5",
          CMC_SENDER_NONCE = "1.3.6.1.5.5.7.7.6",
          CMC_RECIPIENT_NONCE = "1.3.6.1.5.5.7.7.7",
          CMC_ADD_EXTENSIONS = "1.3.6.1.5.5.7.7.8",
          CMC_ENCRYPTED_POP = "1.3.6.1.5.5.7.7.9",
          IPSEC_KP_IKE_INTERMEDIATE = "1.3.6.1.5.5.8.2.2",
          INFOSEC = "2.16.840.1.101.2.1",
          INFOSEC_sdnsSignature = "2.16.840.1.101.2.1.1.1",
          INFOSEC_mosaicKeyManagement = "2.16.840.1.101.2.1.1.10",
          INFOSEC_sdnsKMandSig = "2.16.840.1.101.2.1.1.11",
          INFOSEC_mosaicKMandSig = "2.16.840.1.101.2.1.1.12",
          INFOSEC_SuiteASignature = "2.16.840.1.101.2.1.1.13",
          INFOSEC_SuiteAConfidentiality = "2.16.840.1.101.2.1.1.14",
          INFOSEC_SuiteAIntegrity = "2.16.840.1.101.2.1.1.15",
          INFOSEC_SuiteATokenProtection = "2.16.840.1.101.2.1.1.16",
          INFOSEC_SuiteAKeyManagement = "2.16.840.1.101.2.1.1.17",
          INFOSEC_SuiteAKMandSig = "2.16.840.1.101.2.1.1.18",
          INFOSEC_mosaicUpdatedSig = "2.16.840.1.101.2.1.1.19",
          INFOSEC_mosaicSignature = "2.16.840.1.101.2.1.1.2",
          INFOSEC_mosaicKMandUpdSig = "2.16.840.1.101.2.1.1.20",
          INFOSEC_mosaicUpdateInteg = "2.16.840.1.101.2.1.1.21",
          INFOSEC_sdnsConfidentiality = "2.16.840.1.101.2.1.1.3",
          INFOSEC_mosaicConfidentiality = "2.16.840.1.101.2.1.1.4",
          INFOSEC_sdnsIntegrity = "2.16.840.1.101.2.1.1.5",
          INFOSEC_mosaicIntegrity = "2.16.840.1.101.2.1.1.6",
          INFOSEC_sdnsTokenProtection = "2.16.840.1.101.2.1.1.7",
          INFOSEC_mosaicTokenProtection = "2.16.840.1.101.2.1.1.8",
          INFOSEC_sdnsKeyManagement = "2.16.840.1.101.2.1.1.9",
          NETSCAPE = "2.16.840.1.113730",
          NETSCAPE_CERT_EXTENSION = "2.16.840.1.113730.1",
          NETSCAPE_CERT_TYPE = "2.16.840.1.113730.1.1",
          NETSCAPE_SSL_SERVER_NAME = "2.16.840.1.113730.1.12",
          NETSCAPE_COMMENT = "2.16.840.1.113730.1.13",
          NETSCAPE_BASE_URL = "2.16.840.1.113730.1.2",
          NETSCAPE_REVOCATION_URL = "2.16.840.1.113730.1.3",
          NETSCAPE_CA_REVOCATION_URL = "2.16.840.1.113730.1.4",
          NETSCAPE_CERT_RENEWAL_URL = "2.16.840.1.113730.1.7",
          NETSCAPE_CA_POLICY_URL = "2.16.840.1.113730.1.8",
          DS = "2.5",
          AUTHORITY_KEY_IDENTIFIER = "2.5.29.1",
          BASIC_CONSTRAINTS = "2.5.29.10",
          SUBJECT_KEY_IDENTIFIER = "2.5.29.14",
          KEY_USAGE = "2.5.29.15",
          PRIVATEKEY_USAGE_PERIOD = "2.5.29.16",
          SUBJECT_ALT_NAME2 = "2.5.29.17",
          ISSUER_ALT_NAME2 = "2.5.29.18",
          BASIC_CONSTRAINTS2 = "2.5.29.19",
          KEY_ATTRIBUTES = "2.5.29.2",
          CRL_NUMBER = "2.5.29.20",
          CRL_REASON_CODE = "2.5.29.21",
          REASON_CODE_HOLD = "2.5.29.23",
          DELTA_CRL_INDICATOR = "2.5.29.27",
          ISSUING_DIST_POINT = "2.5.29.28",
          CERT_POLICIES_95 = "2.5.29.3",
          NAME_CONSTRAINTS = "2.5.29.30",
          CRL_DIST_POINTS = "2.5.29.31",
          CERT_POLICIES = "2.5.29.32",
          ANY_CERT_POLICY = "2.5.29.32.0",
          POLICY_MAPPINGS = "2.5.29.33",
          AUTHORITY_KEY_IDENTIFIER2 = "2.5.29.35",
          POLICY_CONSTRAINTS = "2.5.29.36",
          ENHANCED_KEY_USAGE = "2.5.29.37",
          KEY_USAGE_RESTRICTION = "2.5.29.4",
          FRESHEST_CRL = "2.5.29.46",
          LEGACY_POLICY_MAPPINGS = "2.5.29.5",
          SUBJECT_ALT_NAME = "2.5.29.7",
          ISSUER_ALT_NAME = "2.5.29.8",
          SUBJECT_DIR_ATTRS = "2.5.29.9",
          ORGANIZATION_NAME = "2.5.4.10",
          ORGANIZATIONAL_UNIT_NAME = "2.5.4.11",
          TITLE = "2.5.4.12",
          DESCRIPTION = "2.5.4.13",
          SEARCH_GUIDE = "2.5.4.14",
          BUSINESS_CATEGORY = "2.5.4.15",
          POSTAL_ADDRESS = "2.5.4.16",
          POSTAL_CODE = "2.5.4.17",
          POST_OFFICE_BOX = "2.5.4.18",
          PHYSICAL_DELIVERY_OFFICE_NAME = "2.5.4.19",
          TELEPHONE_NUMBER = "2.5.4.20",
          TELEX_NUMBER = "2.5.4.21",
          TELETEXT_TERMINAL_IDENTIFIER = "2.5.4.22",
          FACSIMILE_TELEPHONE_NUMBER = "2.5.4.23",
          X21_ADDRESS = "2.5.4.24",
          INTERNATIONAL_ISDN_NUMBER = "2.5.4.25",
          REGISTERED_ADDRESS = "2.5.4.26",
          DESTINATION_INDICATOR = "2.5.4.27",
          PREFERRED_DELIVERY_METHOD = "2.5.4.28",
          PRESENTATION_ADDRESS = "2.5.4.29",
          COMMON_NAME = "2.5.4.3",
          SUPPORTED_APPLICATION_CONTEXT = "2.5.4.30",
          MEMBER = "2.5.4.31",
          OWNER = "2.5.4.32",
          ROLE_OCCUPANT = "2.5.4.33",
          SEE_ALSO = "2.5.4.34",
          USER_PASSWORD = "2.5.4.35",
          USER_CERTIFICATE = "2.5.4.36",
          CA_CERTIFICATE = "2.5.4.37",
          AUTHORITY_REVOCATION_LIST = "2.5.4.38",
          CERTIFICATE_REVOCATION_LIST = "2.5.4.39",
          SUR_NAME = "2.5.4.4",
          CROSS_CERTIFICATE_PAIR = "2.5.4.40",
          GIVEN_NAME = "2.5.4.42",
          INITIALS = "2.5.4.43",
          DN_QUALIFIER = "2.5.4.46",
          DEVICE_SERIAL_NUMBER = "2.5.4.5",
          COUNTRY_NAME = "2.5.4.6",
          LOCALITY_NAME = "2.5.4.7",
          STATE_OR_PROVINCE_NAME = "2.5.4.8",
          STREET_ADDRESS = "2.5.4.9",
          DSALG = "2.5.8",
          DSALG_CRPT = "2.5.8.1",
          DSALG_RSA = "2.5.8.1.1",
          DSALG_HASH = "2.5.8.2",
          DSALG_SIGN = "2.5.8.3"
      }*/

}
