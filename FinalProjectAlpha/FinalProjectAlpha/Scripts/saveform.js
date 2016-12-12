$(function () {
    $("#save-proj").validate(
        {
            rules: {            //set rules for validation
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
                    rangelength: [100, 500]
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
                    ShortDesc: {
                        required: "Short project description is required.",
                        rangelength: "Your short description should be between 10 and 200 characters."
                    },
                    LongDesc: {
                        rangelength: "Your long description should be between 100 and 500 characters."
                    }
                }
        });

});