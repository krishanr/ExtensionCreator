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

// Get an instance of the controller prefixed by <#= Value("Extension.name") #>
$controller = JControllerLegacy::getInstance('<#= Value("Extension.name") #>');
// Perform the Request task
$controller->execute(JFactory::getApplication()->input->get('task'));
// Redirect if set by the controller
$controller->redirect();

?>
