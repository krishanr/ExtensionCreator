<#@ template inherits="ETemplate" debug="False" language="VB" #>
<?php

class <#= Value("name") #>Class {

function helloWorld() {
        print 'Hello <#= Value("Task.name") #>';
    }

}
?>