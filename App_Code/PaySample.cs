// # Namespaces 
using System;
using System.Collections.Generic;
using System.IO;
// # NuGet Install        
// Visual Studio 2012 and 2010 Command:  
// Install-Package PayPalAdaptivePaymentsSDK    
// Visual Studio 2005 and 2008 (NuGet.exe) Command:  
// install PayPalAdaptivePaymentsSDK    
using PayPal.AdaptivePayments;
using PayPal.AdaptivePayments.Model;
using log4net;

// # Sample for Pay API 
// Use the Pay API operation to transfer funds from a sender's PayPal account to one or more receivers' PayPal accounts. You can use the Pay API operation to make simple payments, chained payments, or parallel payments; these payments can be explicitly approved, preapproved, or implicitly approved.
// This sample code uses AdaptivePayments .NET SDK to make API call. You can
// download the SDK [here](https://github.com/paypal/sdk-packages/tree/gh-pages/adaptivepayments-sdk/dotnet)
public class PaySample
{
    // # Static constructor for configuration setting
    static PaySample()
    {
        // Load the log4net configuration settings from Web.config or App.config        
        log4net.Config.XmlConfigurator.Configure();
    }

    // Logs output statements, errors, debug info to a text file       
    private static ILog logger = LogManager.GetLogger(typeof(PaySample));

    // # Payment
    public PayRequest Payment()
    {
        // # PayRequest
        // The code for the language in which errors are returned
        RequestEnvelope envelopeRequest = new RequestEnvelope();
        envelopeRequest.errorLanguage = "en_US";

        List<Receiver> listReceiver = new List<Receiver>();

        // Amount to be credited to the receiver's account
        Receiver receive = new Receiver(Convert.ToDecimal("4.00"));

        // A receiver's email address
        receive.email = "abc@paypal.com";
        listReceiver.Add(receive);
        ReceiverList listOfReceivers = new ReceiverList(listReceiver);

        // PayRequest which takes mandatory params:
        //  
        // * `Request Envelope` - Information common to each API operation, such
        // as the language in which an error message is returned.
        // * `Action Type` - The action for this request. Possible values are:
        // * PAY - Use this option if you are not using the Pay request in
        // combination with ExecutePayment.
        // * CREATE - Use this option to set up the payment instructions with
        // SetPaymentOptions and then execute the payment at a later time with
        // the ExecutePayment.
        // * PAY_PRIMARY - For chained payments only, specify this value to delay
        // payments to the secondary receivers; only the payment to the primary
        // receiver is processed.
        // * `Cancel URL` - URL to redirect the sender's browser to after
        // canceling the approval for a payment; it is always required but only
        // used for payments that require approval (explicit payments)
        // * `Currency Code` - The code for the currency in which the payment is
        // made; you can specify only one currency, regardless of the number of
        // receivers
        // * `Recevier List` - List of receivers
        // * `Return URL` - URL to redirect the sender's browser to after the
        // sender has logged into PayPal and approved a payment; it is always
        // required but only used if a payment requires explicit approval
        PayRequest requestPay = new PayRequest(envelopeRequest, "PAY", "http://localhost/cancel", "USD", listOfReceivers, "http://localhost/return");
        return requestPay;
    }

    // # Simple Payment
    public PayRequest SimplePayment()
    {
        PayRequest requestPay = Payment();
        return requestPay;
    }

    // # Chained Payment
    // `Note:
    // For chained Payment all the above mentioned request parameters in
    // SimplePay() are required, but in receiverList alone we have to make
    // receiver as Primary Receiver or Not Primary Receiver`
    public PayRequest ChainPayment()
    {
        // Payment operation
        PayRequest requestPay = Payment();

        List<Receiver> listReceiver = new List<Receiver>();

        // Amount to be credited to the receiver's account
        Receiver receiverA = new Receiver(Convert.ToDecimal("4.00"));

        // A receiver's email address
        receiverA.email = "com.swamthat1@testing.com";

        // Set to true to indicate a chained payment; only one receiver can be a
        // primary receiver. Omit this field, or set it to false for simple and
        // parallel payments.
        receiverA.primary = true;
        listReceiver.Add(receiverA);

        // Amount to be credited to the receiver's account
        Receiver receiverB = new Receiver(Convert.ToDecimal("2.00"));

        // A receiver's email address
        receiverB.email = "com.swamthat3@testing.com";

        // Set to true to indicate a chained payment; only one receiver can be a
        // primary receiver. Omit this field, or set it to false for simple and
        // parallel payments.
        receiverB.primary = false;
        listReceiver.Add(receiverB);

        ReceiverList receiverList = new ReceiverList(listReceiver);
        requestPay.receiverList = receiverList;
        return requestPay;
    }

    // # Parallel Payment
    // `Note:
    // For parallel Payment all the above mentioned request parameters in
    // SimplePay() are required, but in receiverList we can have multiple
    // receivers`
    public PayRequest ParallelPayment()
    {
        // Payment operation
        PayRequest requestPay = Payment();

        List<Receiver> receiverLst = new List<Receiver>();

        // Amount to be credited to the receiver's account
        Receiver receiverA = new Receiver(Convert.ToDecimal("4.00"));

        // A receiver's email address
        receiverA.email = "abc@paypal.com";
        receiverLst.Add(receiverA);

        // Amount to be credited to the receiver's account
        Receiver receiverB = new Receiver(Convert.ToDecimal("2.00"));

        // A receiver's email address
        receiverB.email = "xyz@paypal.com";
        receiverLst.Add(receiverB);

        ReceiverList receiverList = new ReceiverList(receiverLst);
        requestPay.receiverList = receiverList;

        // IPN URL
        //  
        // * PayPal Instant Payment Notification is a call back system that is initiated when a transaction is completed        
        // * The transaction related IPN variables will be received on the call back URL specified in the request       
        // * The IPN variables have to be sent back to the PayPal system for validation, upon validation PayPal will send a response string "VERIFIED" or "INVALID"     
        // * PayPal would continuously resend IPN if a wrong IPN is sent        
        requestPay.ipnNotificationUrl = "http://IPNhost";

        return requestPay;
    }

    // # Pay API Operations
    // Use the Pay API operations to transfer funds from a sender’s PayPal account to one or more receivers’ PayPal accounts. You can use the Pay API operation to make simple payments, chained payments, or parallel payments; these payments can be explicitly approved, preapproved, or implicitly approved. 
    public PayResponse PayAPIOperations(PayRequest reqPay)
    {
        // Create the PayResponse object
        PayResponse responsePay = new PayResponse();

        try
        {
            // Create the service wrapper object to make the API call
            AdaptivePaymentsService service = new AdaptivePaymentsService();

            // # API call
            // Invoke the Pay method in service wrapper object
            responsePay = service.Pay(reqPay);

            if (responsePay != null)
            {
                // Response envelope acknowledgement
                string acknowledgement = "Pay API Operation - ";
                acknowledgement += responsePay.responseEnvelope.ack.ToString();
                logger.Info(acknowledgement + "\n");
                Console.WriteLine(acknowledgement + "\n");

                // # Success values
                if (responsePay.responseEnvelope.ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
                {
                    // The pay key, which is a token you use in other Adaptive
                    // Payment APIs (such as the Refund Method) to identify this
                    // payment. The pay key is valid for 3 hours; the payment must
                    // be approved while the pay key is valid.
                    logger.Info("Pay Key : " + responsePay.payKey + "\n");
                    Console.WriteLine("Pay Key : " + responsePay.payKey + "\n");

                    // Once you get success response, user has to redirect to PayPal
                    // for the payment. Construct redirectURL as follows,
                    // `redirectURL=https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_ap-payment&paykey="
                    // + responsePay.payKey;`
                }
                // # Error Values 
                else
                {
                    List<ErrorData> errorMessages = responsePay.error;
                    foreach (ErrorData error in errorMessages)
                    {
                        logger.Debug(error.message);
                        Console.WriteLine(error.message + "\n");
                    }
                }
            }
        }
        // # Exception log    
        catch (System.Exception ex)
        {
            // Log the exception message       
            logger.Debug("Error Message : " + ex.Message);
            Console.WriteLine("Error Message : " + ex.Message);
        }
        return responsePay;
    }
}
