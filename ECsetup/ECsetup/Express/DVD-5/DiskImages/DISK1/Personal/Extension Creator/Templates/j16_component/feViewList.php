<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * <#= Value("Extension.name") #> View for com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 * Created with Marco's Component Creator for Joomla! 1.6
 * http://www.mmleoni.net/joomla-component-builder
 *
 */

jimport( 'joomla.application.component.view');

/**
 * HTML View class for the <#= Value("Extension.name") #> Component
 *
 * @package		<#= Value("Extension.name") #>
 * @subpackage	Components
 */
class <#= Value("Extension.name") #>View<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JView
{
	function display($tpl = null){
		$app =& JFactory::getApplication();
		/*
		$params =& JComponentHelper::getParams( 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>' );
		$params =& $app->getParams( 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>' );	
		$dummy = $params->get( 'dummy_param', 1 ); 
		*/
	
		$data =& $this->get('Data');
		$this->assignRef('data', $data);
		
		$pagination =& $this->get('Pagination');
		$this->assignRef('pagination', $pagination);

		parent::display($tpl);
	}
}
?>
