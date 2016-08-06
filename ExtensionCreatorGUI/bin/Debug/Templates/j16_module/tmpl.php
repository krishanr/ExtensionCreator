<#@ template inherits="ETemplate" language="VB" #>
<?php
// no direct access
defined('_JEXEC') or die;
//jimport('mylibrary.html.html.mybehavior');

//Load CSS and JS
$basePath = JURI::root(true).'/media/'.$module->module;
$doc = JFactory::getDocument();
//$doc->addStyleSheet($basePath.'/css/CSSFILENAME.css');
//$doc->addScript($basePath.'/js/JSFILENAME.js');
?>