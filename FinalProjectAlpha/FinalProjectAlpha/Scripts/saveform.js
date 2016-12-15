
///<reference path="jquery.validate.js" />
/// <reference path="jquery.validate.unobtrusive.min.js" />
///<reference path="jquery-3.1.1.intellisense.js" />
///<reference path="respond.js" />


$(function () {
    validator = $('#save-proj').validate({

        rules: { //set rules for validation
            Link: {
                required: true,         //causes the form field named "Link" to be required
                url: true,         //requires valid URL input in the form field named "Link" (JQuery checks and flags on submit if it is not)
                //regexp may be redundant.  is a pattern the input should conform to (permits http, https, etc.)
                regexp: /^(http|https):\/\/[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i
            },
            RepoLink: {
                required: false,        //causes the form field named "RepoLink" to not be required
                url: true,//requires URL input in the form field named "RepoLink" (JQuery checks and flags on submit if it is not)
                //regexp may be redundant.  is a pattern the input should conform to (permits ftp and https, etc.)
                regexp: /^(http|https|ftp):\/\/[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i
            },
            ShortDesc: {
                required: true,              //causes the form field named "ShortDesc" to not be required
                rangelength: [10, 200]      //rule controlling upper and lower limits of character number in "ShortDesc"
            },
            LongDesc: {
                required: false,             //causes the form field named "ShortDesc" to not be required
                rangelength: [50, 500]      //rule controlling upper and lower limits of character number in "ShortDesc"
            }
        },
        messages:
            {                    //error messages (to be displayed if the above rules are not met.  displayed when focus of field is lost (after some input only)
                Link: {
                    required: "Your Project URL is required.",      //message displayed if "Link" input is left blank
                    url: "Please enter a valid, working URL."
                },
                RepoLink: {
                    url: "Please enter a valid, working URL."
                },
                //If user enters nothing in the required field, and included is the ranges for short and ong description
                ShortDesc: {
                    required: "Short project description is required.",      //message displayed if "ShortDesc" input is left blank
                    rangelength: "Your short description should be between 10 and 200 characters."  //message displayed if the character range requirements are not met
                },
                LongDesc: {

                    rangelength: "Your long description should be between 50 and 500 characters."   //message displayed if the character range requirements are not met
                }
            }

    });
    //$("#save-proj").validate({
    //error: function(label) {
    //    $(this).addClass("error");
    //},
});

