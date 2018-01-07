<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * <#= Value("Extension.name") #> View for com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

jimport( 'joomla.application.component.view');

/**
 * HTML View class for the <#= Value("Extension.name") #> Component
 *
 * @package	Joomla.Components
 * @subpackage	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>View<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JView
{
	function display($tpl = null)
	{
		$this->state 		= $this->get('State');
		$this->items 		= $this->get('Items');

		// Check for errors.
		if (count($errors = $this->get('Errors'))) {
			JError::raiseWarning(500, implode("\n", $errors));
			return false;
		}

		$this->params = &$this->state->params;

		parent::display($tpl);
	}
}
?>
