# Track generic Forms submission
The following example code adds a forms submission actor, which runs after a form has been submitted.

It looks for fields in the form for email, name and phone (based on the name of the form field), and posts these to the form.

```cs
using EPiServer.Forms.Core.PostSubmissionActor;
using EPiServer.Forms.Core.PostSubmissionActor.Internal;
using EPiServer.Forms.Helpers.Internal;
using OdpTracking;
using OdpTracking.Dto;
using OdpTracking.Extensions;

namespace YourSite.Business.Tracking
{
    /// <summary>
    /// The Forms engine will find all actors and invoke them in turn
    /// </summary>
    public class OdpTrackFormSubmission : PostSubmissionActorBase, ISyncOrderedSubmissionActor
    {

        /// <summary>
        /// Ascending Order of running actors. Set Order smaller than 1000 to make actor run before saving data to storage.
        /// </summary>
        public int Order => 1001; // after save

        private IOdpServerSideTracker OdpServerSideTracker { get; }

        public OdpTrackFormSubmission(IOdpServerSideTracker odpServerSideTracker)
        {
            OdpServerSideTracker = odpServerSideTracker;
        }

        public override object Run(object input)
        {
            // Actor which wants to cancel the submission process should return this object by set CancelSubmit to true.
            // And set ErrorMessage to show the reason for cancellation on UI.
            var result = new SubmissionActorResult { CancelSubmit = false, ErrorMessage = string.Empty };

            string vuid = this.HttpRequestContext.Cookies.GetVuid();
            if (string.IsNullOrEmpty(vuid) == false)
            {
                OdpDtoProfile dtoProfile = new OdpDtoProfile { Vuid = vuid };
                dtoProfile.Email = FindFormValueFromFieldName("email", "epost", "e-post");
                dtoProfile.FirstName = FindFormValueFromFieldName("firstname", "name", "fornavn");
                dtoProfile.LastName = FindFormValueFromFieldName("lastname", "surname", "etternavn");
                dtoProfile.Phone = FindFormValueFromFieldName("mobil", "phone");

                if(string.IsNullOrEmpty(dtoProfile.Email) == false)
                {
                    OdpServerSideTracker.UpdateProfile(dtoProfile);
                }

                // Track that the user filled in the form
                var formContainerBlock = this.FormIdentity.GetFormBlock();
                OdpDtoEvent dtoEvent = new OdpDtoEvent { 
                    Type = "web_form", 
                    Action = "submitted", 
                    Identifiers = new OdpDtoIdentifiers { Vuid = vuid } };

                // Dictionary<string, string> data = new Dictionary<string, string>
                // {
                //     { "campaign", formContainerBlock.Title }
                // };
                //
                //dtoEvent.Data = data;

                OdpServerSideTracker.TrackEvent(dtoEvent);

            }

            return result;
        }

        /// <summary>
        /// Loops through a list of field names and looks for fields in the form that
        /// contains the field name. Returns the value of the first one found.
        /// </summary>
        /// <param name="fieldNames">List of field names to look for</param>
        /// <returns>The value of the first form field that contains the field name, null if none found.</returns>
        private string FindFormValueFromFieldName(params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                var element = SubmissionFriendlyNameInfos.FirstOrDefault(
                    x => x.FriendlyName.Contains(fieldName, StringComparison.InvariantCultureIgnoreCase));
                if (element != null)
                {
                    var formValue = SubmissionData.Data[element.ElementId].ToString();
                    if (formValue != null)
                    {
                        return formValue;
                    }

                }
            }

            return null;
            
        }
    }
}
```
