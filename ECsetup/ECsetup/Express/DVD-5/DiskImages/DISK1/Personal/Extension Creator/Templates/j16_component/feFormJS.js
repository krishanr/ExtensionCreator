<#@ template inherits="ETemplate" language="VB" #>
var j = jQuery.noConflict();

j.validator.addMethod("letterswithbasicpunc", function(value, element) {
        return this.optional(element) || /^[a-z-.,()!'\"\s\?]+$/i.test(value);
    }, "Letters or punctuation only please."
);

j.validator.addMethod("lettersnumberswithbasicpunc", function(value, element) {
    return this.optional(element) || /^[a-z0-9-.,()!;:%\$'\"\s\?]+$/i.test(value);
}, "Letters, numbers or punctuation only please."
);

j.validator.addMethod("postalcode", function(value, element) {
        var regexObj = {
            canada         : /^[ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ]( )?\d[ABCEGHJKLMNPRSTVWXYZ]\d$/i ,
            usa            : /^\d{5}(-\d{4})?$/ 
        };
        var CanadaRegexp   = new RegExp(regexObj.canada);
        var UsaRegexp      = new RegExp(regexObj.usa);
        return this.optional(element) || CanadaRegexp.test(value) || UsaRegexp.test(value);
    }, "Please enter a valid postal code."
);

j.validator.addClassRules({
	  requiredText: {
	    required: true,
	    lettersnumberswithbasicpunc: true,
	    minlength: 2
	  },
	  optionalText: {
		  lettersnumberswithbasicpunc: true
	  }
});

j(document).ready(function() { 
    j("#<#= Value("Task.nameObject") #>Form").validate({
        rules: {
        },
        messages: {
        },
        errorPlacement: function(error, element) {
        	/* if (element.is(":radio") && element.attr('name') == 'jform[existing]') {
            	error.appendTo( j('#jform_existing') );
            } */
                error.appendTo( element.parent() );
        },
        submitHandler: function() {
            if (j("#<#= Value("Task.nameObject") #>Form").hasClass("submitted")) {
                alert("Please wait untill the form is finished submitting.");
           } else {
               j("#<#= Value("Task.nameObject") #>Form").addClass("submitted");              
               document.<#= Value("Task.nameObject") #>Form.submit();                    
           }
        }
    });
    
    /*j("#jform_phone").mask("(999) 999-9999"); 
    j('#jform_date').datepicker({ dateFormat: 'yy-mm-dd' }); */
});