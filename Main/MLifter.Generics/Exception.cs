using System;
using System.Collections.Generic;
using System.Web;

namespace MLifter.Generics
{
    #region Activation Service Exceptions
    /// <summary>
    /// This exception is thrown when there was a problem with the database
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-11</remarks>
    public class DatabaseOpenException : Exception
    {
        public DatabaseOpenException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }

    /// <summary>
    /// This exception is thrown when there was a problem with the database
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-11</remarks>
    public class DatabaseCloseException : Exception
    {
        public DatabaseCloseException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }

    /// <summary>
    /// This exception is thrown when there was a problem with the database
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-11</remarks>
    public class DatabaseExecuteException : Exception
    {
        public DatabaseExecuteException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }

    /// <summary>
    /// This exception is thrown when there was a problem with the database
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-11</remarks>
    public class DatabaseCreateCommandException : Exception
    {
        public DatabaseCreateCommandException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }

    /// <summary>
    /// This exception is thrown when there was a problem with the license activation
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-11</remarks>
    public class LicenseAlreadyActivatedException : Exception
    {
        public LicenseAlreadyActivatedException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }


    /// <summary>
    /// This exception is thrown when there was a problem with the license activation
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-11</remarks>
    public class LicenseNotInDbException : Exception
    {
        public LicenseNotInDbException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }

    /// <summary>
    /// This exception is thrown when there was a problem with the license activation
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-11</remarks>
    public class CouldNotSetActivatedFlagException : Exception
    {
        public CouldNotSetActivatedFlagException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }

    /// <summary>
    /// This exception is thrown when too many activation-attempts were made by a certain IP-address
    /// </summary>
    /// <remarks>Documented by Dev04, 2009-02-12</remarks>
    public class BruteForceAttackException : Exception
    {
        public BruteForceAttackException(string message)
            : base(message) { }

        public override string Message
        {
            get
            {
                return this.GetType().ToString() + ": " + base.Message;
            }
        }
    }

    #endregion

    /// <summary>
    /// This exception is thrown when the signature of the received activation key is invalid.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-11</remarks>
    public class SignatureInvalidException : Exception { }

    /// <summary>
    /// This exception is thrown when a protected learning module is tryed to open without or with the wrong password.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-12</remarks>
    public class ProtectedLearningModuleException : Exception { }

    /// <summary>
    /// Error during generating the Machine ID.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-13</remarks>
    public class MachineIDGenerationException : Exception
    {
        public MachineIDGenerationException(Exception innerExeption) : base(string.Empty, innerExeption) { }
    }

    /// <summary>
    /// This exception is thrown when the CAL count is exceeded.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-13</remarks>
    public class ClientAccessLicenceCountExceededException : Exception { }

    /// <summary>
    /// This exception is thrown if no LM Id is found within the eDb file.
    /// </summary>
    /// <remarks>Documented by Dev10, 2009-18-02</remarks>
    public class NoLMIdsFoundException : Exception { }

    /// <summary>
    /// This exception is thrown if there is no connection available where the user has rights to write.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-27</remarks>
    public class NoWritableConnectionAvailableException : Exception { }

    /// <summary>
    /// This exception is thrown if a invalid config file 
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-03-03</remarks>
    public class InvalidConfigFileException : Exception { }

    /// <summary>
    /// Exception is thrown when the given credetials are invalid.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-11-14</remarks>
    public class InvalidCredentialsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        public InvalidCredentialsException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception is thrown when the given credetials (username) are invalid.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-11-14</remarks>
    public class InvalidUsernameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUsernameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        public InvalidUsernameException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception is thrown when the given credetials (password) are invalid.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-04</remarks>
    public class InvalidPasswordException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        public InvalidPasswordException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception is thrown when the selected authentication mode is not valid for this user.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-11-14</remarks>
    public class WrongAuthenticationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongAuthenticationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        public WrongAuthenticationException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception is thrown when the selected authentication mode is not allowed on this server.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-11-14</remarks>
    public class ForbiddenAuthenticationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenAuthenticationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        public ForbiddenAuthenticationException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception is thrown in case the server could not create the new user session.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-11-13</remarks>
    public class UserSessionCreationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionCreationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        public UserSessionCreationException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when a learning module cannot be closed.
    /// </summary>
    /// <remarks>Documented by Dev09, 2009-03-05</remarks>
    public class CouldNotCloseLearningModuleException : Exception { }

    /// <summary>
    /// Exception thrown when a learning module cannot be opened.
    /// </summary>
    /// <remarks>Documented by Dev09, 2009-03-05</remarks>
    public class CouldNotOpenLearningModuleException : Exception { }

    /// <summary>
    /// Exception thrown when a web or db server is offline.
    /// </summary>
    /// <remarks>Documented by Dev09, 2009-03-05</remarks>
    public class ServerOfflineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerOfflineException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-04-28</remarks>
        public ServerOfflineException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerOfflineException"/> class.
        /// </summary>
        /// <param name="innerExeption">The inner exeption.</param>
        /// <remarks>Documented by Dev03, 2009-04-28</remarks>
        public ServerOfflineException(Exception innerExeption) : base(innerExeption.Message, innerExeption) { }
    }

    /// <summary>
    /// Exception thrown if a synchronization failed.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-03-24</remarks>
    public class SynchronizationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationFailedException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-04-28</remarks>
        public SynchronizationFailedException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationFailedException"/> class.
        /// </summary>
        /// <param name="innerExeption">The inner exeption.</param>
        /// <remarks>Documented by Dev03, 2009-04-28</remarks>
        public SynchronizationFailedException(Exception innerExeption) : base(innerExeption.Message, innerExeption) { }
    }

    /// <summary>
    /// Occures when trying to get a web uri form a webserver with port 0 (=invalid).
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-04-07</remarks>
    public class WebServerPortNullException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebServerPortNullException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-04-28</remarks>
        public WebServerPortNullException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebServerPortNullException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev03, 2009-04-28</remarks>
        public WebServerPortNullException(string message) : base(message) { }
    }

    /// <summary>
    /// Occurs when the users activates a key, but for the wrong learning module
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-04-29</remarks>
    public class WrongKeyActivatedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongKeyActivatedException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-29</remarks>
        public WrongKeyActivatedException() : base() { }
    }

    /// <summary>
    /// Exception is thrown in case the user session was invalidated on the server.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-11-13</remarks>
    public class UserSessionInvalidException : Exception { public UserSessionInvalidException(string message) : base(message) { } }

    /// <summary>
    /// Occurs when there is not enought disk space.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-05-15</remarks>
    public class NotEnoughtDiskSpaceException : Exception { }

    /// <summary>
    /// Occurs when the connection is lost.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-05-15</remarks>
    public class ConnectionLostException : Exception { }
}
