<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<?php
/**
 * <#= Value("Extension.name") #> Controller for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */

// No direct access
defined( '_JEXEC' ) or die( 'Restricted access' );

jimport('joomla.application.component.controller');

/**
 * <#= Value("Extension.name") #> Model
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>Controller extends JController
{
	/**
	 * @var		string	The default view.
	 * @since	1.6
	 */
	protected $default_view = '<#= Value("Extension.defaultView") #>';
	
	/**
	 * Method to display a view.
	 *
	 * @param	boolean			If true, the view output will be cached
	 * @param	array			An array of safe url parameters and their variable types, for valid values see {@link JFilterInput::clean()}.
	 *
	 * @return	JController		This object to support chaining.
	 * @since	1.5
	 */
	public function display($cachable = false, $urlparams = false)
	{
		<# If JExt.GetTasksWithNoParams("Helpers").Count() > 0 #>
		require_once JPATH_COMPONENT.'/helpers/<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>.php';

		// Load the submenu.
		<#= Value("Extension.name") #>Helper::addSubmenu($this->input->get('view', '<#= Value("Extension.defaultView") #>'));
		
		<# End If #>
		parent::display($cachable); 
		return $this;
	}
}