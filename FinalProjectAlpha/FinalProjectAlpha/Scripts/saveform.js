
﻿/// <reference path="jquery.validate.js" />
/// <reference path="jquery-3.1.1.intellisense.js" />
/// <reference path="respond.js" />


JQuery (function ($) {
    var validator =  $('#save-proj').validate ({
      
        rules: { //set rules for validation
            Link: {
                required: true,
                url: true,
                regexp: /^(http|https|ftp):\/\/[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i
            },
            RepoLink: {
                required: false,
                url: true,
                regexp: /^(http|https|ftp):\/\/[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i
            },
            ShortDesc: {
                required: true,
                rangelength: [10, 200]
            },
            LongDesc: {
                required: false,
                rangelength: [50, 500]
            }
        },
        messages:
            { //error messages
                Link: {
                    required: "Your Project URL is required.",
                    url: "Please enter a valid, working URL."
                },
                RepoLink: {
                    url: "Please enter a valid, working URL."
                },
                //If user enters nothing in the required field, and included is the ranges for short and ong description
                ShortDesc: {
                    required: "Short project description is required.",
                    rangelength: "Your short description should be between 10 and 200 characters."
                },
                LongDesc: {

                    rangelength: "Your long description should be between 50 and 500 characters."
                }
            },
        highlight: function (element) {
            $(element).parent().addClass('error')
        },
        unhighlight: function (element) {
            $(element).parent().removeClass('error')
        }
    });
})

             validator.resetForm();
             validator.showErrors({
        "firstname": "I know that your firstname is Pete, Pete!"
    });

        
    //errorPlacement: function(error, element) {
    //    error.insertAfter('.form-group'); //So i putted it after the .form-group so it will not include to your append/prepend group.
    //}; 
    //highlight: function(Link) {
    //    $(element).closest('.form-group').addClass('has-error');
    //};
    //unhighlight: function(element) {
    //    $(element).closest('.form-group').removeClass('has-error');
    //}

