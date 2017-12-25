<#@ template inherits="ETemplate" language="VB" #>
<#@ import namespace="System.Linq" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<?php
/**
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 *
 */

// No direct access
defined( '_JEXEC' ) or die( 'Restricted access' );

// Access check.
if (!JFactory::getUser()->authorise('core.manage', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')){
	return JError::raiseWarning(404, JText::_('JERROR_ALERTNOAUTHOR'));
}

<# 'Check if Helpers task was run.'
Dim HelpersCn = Aggregate Task In JExt.TaskDictionary
Where Task.Value.item("TaskName") Like "Helpers"
Into Count()
#>
<# If HelpersCn > 0 #>
JLoader::register('<#= Value("Extension.name") #>Helper', __DIR__ . '/helpers/<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>.php');
<# End If #>

$controller = JControllerLegacy::getInstance('<#= Value("Extension.name") #>');
// Perform the Request task
$controller->execute(JFactory::getApplication()->input->get('task'));
// Redirect if set by the controller
$controller->redirect();