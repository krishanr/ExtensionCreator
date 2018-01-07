<#@ template inherits="ETemplate" debug="False" language="VB" #>
<?php
/**
 * @version		<#= Value("Extension.version") #>
 * @package		Joomla.<#= Value("Extension.client") #>
 * @subpackage	mod_<#= Value("Extension.name") #>
 * @copyright	<#= Value("Extension.copyright") #>
 * @license		<#= Value("Extension.license") #>
 * <# Ext.GenText(Value("Extension.fullName"), Value("Extension.label"), Ext.LangFiles.Item("item")) #>
 */

// no direct access 
defined('_JEXEC') or die;

// Include the syndicate functions only once
require_once dirname(__FILE__).'/helper.php';

$moduleclass_sfx = htmlspecialchars($params->get('moduleclass_sfx'));

require JModuleHelper::getLayoutPath('<#= Value("Extension.fullName") #>', $params->get('layout', 'default'));
