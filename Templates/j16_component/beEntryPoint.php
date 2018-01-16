<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

// No direct access
defined( '_JEXEC' ) or die( 'Restricted access' );

// Access check.
if (!JFactory::getUser()->authorise('core.manage', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')){
	return JError::raiseWarning(404, JText::_('JERROR_ALERTNOAUTHOR'));
}

// Include dependencies
jimport('joomla.application.component.controller');

$controller = JController::getInstance('<#= Value("Extension.name") #>');
$controller->execute(JRequest::getCmd('task'));
$controller->redirect();