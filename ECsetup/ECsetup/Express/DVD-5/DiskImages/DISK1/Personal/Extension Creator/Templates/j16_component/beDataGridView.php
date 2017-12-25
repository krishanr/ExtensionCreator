<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<# Dim JComponentDataTable = JExt.GetActiveTaskCollection().Item("JDataTable").JComponentDataTable #>
<# Dim row #>
<?php
/**
 * <#= Value("Extension.name") #> View for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */

// No direct access
defined( '_JEXEC' ) or die( 'Restricted access' );

jimport( 'mylibrary.application.component.datagridview' );

/**
 * <#= Value("Extension.name") #> DataGridView
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>View<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JDataGridView
{

	/**
	 * Add the page title and toolbar.
	 *
	 * @since	1.6
	 */
	protected function addToolbar()
	{
		$user		= JFactory::getUser();
		<# Dim myTempNameObject as string = StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #>
		<# Dim myTempNameObjectList as string = StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #>
		<# Dim ManagersKey as string = JExt.GenText("manager " & myTempNameObjectList, myTempNameObject & " Manager: " & myTempNameObjectList) #>
		JToolBarHelper::title( JText::_( '<#= ManagersKey #>' ), 'generic.png' );
		<# if StrConv(Value("Task.canCreate"), VbStrConv.Lowercase) = "true" Then #>
		if ($user->authorise('core.create', $this->option)) {
			JToolBarHelper::addNew('<#= Value("Task.nameObject") #>.add','JTOOLBAR_NEW');
		}
		<# End If #>
		
		<# if StrConv(Value("Task.canEdit"), VbStrConv.Lowercase) = "true" Then #>
		if ($user->authorise('core.edit', $this->option)) {
			JToolBarHelper::editList('<#= Value("Task.nameObject") #>.edit','JTOOLBAR_EDIT');
		}
		<# End If #>
				
		if($user->authorise('core.delete', $this->option)) {
			JToolBarHelper::deleteList('', '<#= Value("Task.nameObjectList") #>.delete');	
		}
		
		if( (isset(current($this->items)->published)) && ($user->authorise('core.edit.state', $this->option)) ){
			JToolBarHelper::divider();
			JToolBarHelper::publishList('<#= Value("Task.nameObjectList") #>.publish');
			JToolBarHelper::unpublishList('<#= Value("Task.nameObjectList") #>.unpublish');
		}
		
		if($user->authorise('core.admin', $this->option)){
			JToolBarHelper::divider();
			JToolBarHelper::preferences($this->option);
		}
	}	
	
	public function getLinks() {
		$tableKey = $this->tableKey;
		$links = array();
		<# For Each row In JComponentDataTable.Rows #>
		<#     If row.Linkable = True Then #>
		$links['<#= row.Field #>'] = array();
		<#     End If #>
		<# Next #>
		foreach($this->items as $item) {
		<# For Each row In JComponentDataTable.Rows #>
		<#     If row.Linkable = True Then #>
			$link = new JObject;
			$link->url = JRoute::_("index.php?option=$this->option&task=<#= Value("Task.nameObject") #>.edit&id=".$item->$tableKey);
			$links['<#= row.Field #>'][$item->$tableKey] = clone $link;
		<#     End If #>
		<# Next #>
		}
		return $links;
	}
}