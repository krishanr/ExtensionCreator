<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * <#= Value("Extension.name") #> entry point file for <#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

// no direct access
defined('_JEXEC') or die('Restricted access');

// import joomla controller library
jimport('joomla.application.component.controller');

// Get an instance of the controller prefixed by <#= Value("Extension.name") #>
$controller = JController::getInstance('<#= Value("Extension.name") #>');

// Perform the Request task
$controller->execute(JRequest::getCmd('task'));
// Redirect if set by the controller
$controller->redirect();

?>
