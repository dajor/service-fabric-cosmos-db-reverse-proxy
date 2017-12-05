using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonBase.Models
{
    public static class ErrorCodes
    {
        #region All DBOperationErrorCodes range 1000 - 1999

        public const int DBInvalidDocument = 1001;
        public const int DBCollectionFull = 1002;
        public const int DBDocumentNotFound = 1003;
        public const int DBDocumentSizeExceedsMaximumSize = 1004;
        public const int DBUncommonHttpStatusFound = 1005;
        public const int DBDocumentNotFoundOrAttachmentIsNull = 1006;
        public const int DBNoAttachmentLinkFoundForRemovingAttachment = 1007;
        public const int DBGeneralSqlQueryError = 1008;


        public const int EliteError = 1337;

        //All non leading Database name and collection name errors.
        public const int DBTriedToRetrieveDocumentWithLackOfRights = 1500;
        public const int DBTriedToManipulateDocumentWithLackOfRights = 1501;

        #endregion

        #region All SystemErrorCodes range 2000 - 2999

        //Codes related to mailing and other communications.

        #endregion

        #region All OperationErrorCodes range 3000 - 3999

        #region Bad requests 3000 - 3599

        #endregion region

        #region Internal Server Errors 3600 - 3999

        public const int ArgumentNullOrEmpty = 3605;
        public const int ArgumentWrongType = 3606;

        #endregion

        #endregion



        private static readonly Dictionary<int, string> ErrorCodeMessageMapping = new Dictionary<int, string>
        {
            #region All DBOperationErrorMessages

            { DBInvalidDocument,                            "Database: {0}, collection {1}. >>> Invalid document: {2}." },
            { DBCollectionFull,                             "Database: {0}, collection {1}. >>> The collection is full." },
            { DBDocumentNotFound,                           "Database: {0}, collection {1}. >>> Document with id {2} was requested, but not found." },
            { DBDocumentSizeExceedsMaximumSize,             "Database: {0}, collection {1}. >>> The document size exceeds the maximum size. Document: {2}." },
            { DBUncommonHttpStatusFound,                    "Database: {0}, collection {1}. >>> An uncommon HttpStatusCode is found: {2} for document: {3}." },
            { DBDocumentNotFoundOrAttachmentIsNull,         "Database: {0}, collection {1}. >>> Failed to save attachment because document with id {0} not found or attachment is null." },
            { DBNoAttachmentLinkFoundForRemovingAttachment, "Database: {0}, collection {1}. >>> Failed to save attachment because document with id {0} not found or attachment is null." },
            { DBTriedToRetrieveDocumentWithLackOfRights,    "User {0} ({1}) is logged in for {2} but tries to retrieve document with id {3} which belongs to organization {4}." },
            { DBTriedToManipulateDocumentWithLackOfRights,  "User {0} ({1}) is logged in for {2} but tries to manipulate document with id {3} which belongs to organization {4}." },
            { DBGeneralSqlQueryError,                       "Database: {0}, collection {1}. >>> General query error: {2}" },


            { EliteError,                                   "An Elite Error has occured." },

            #endregion

            #region All OperationErrorMessages

          
            { ArgumentNullOrEmpty,                                          "Argument {0} was null or empty." },
            { ArgumentWrongType,                                            "Argument '{0}' should be of type {1} but found {2}." },
            #endregion
        };



        /// <summary>
        /// Returns the message linked to the code.
        /// </summary>
        /// <param name="code">The code required for retrieving the message.</param>
        /// <returns>The message.</returns>
        public static string GetMessageByCode(int code)
        {
            return ErrorCodeMessageMapping.ContainsKey(code) ?
                $"{ErrorCodeMessageMapping[code]}" :
                "No message specified for this code.";
        }

        /// <summary>
        /// Adds the code to the front of the message and then returns the error message.
        /// </summary>
        /// <param name="code">Code required for retrieving the message.</param>
        /// <returns>The message with the code.</returns>
        public static string GetMessageByCodeWithCode(int code)
        {
            return $"ERROR [{code}]: {GetMessageByCode(code)}";
        }
    }
}