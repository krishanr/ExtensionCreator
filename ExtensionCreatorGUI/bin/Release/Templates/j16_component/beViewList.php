<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
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

jimport( 'joomla.application.component.view' );

/**
 * <#= Value("Extension.name") #> View
 *
 * @package    Joomla.Components
 * @subpackage 	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>View<#= StrConv(Value("Task.nameObjectList"), VbStrConv.ProperCase) #> extends JView
{
	protected $form;
	protected $items;
	protected $pagination;
	protected $state;

	/**
	 * Display the view
	 *
	 * @return	void
	 */
	public function display($tpl = null)
	{
		$this->state		= $this->get('State');
		$this->form		    = $this->get('Form');
		$this->items		= $this->get('Items');
		$this->pagination	= $this->get('Pagination');
		
		// Check for errors.
		if (count($errors = $this->get('Errors'))) {
			JError::raiseError(500, implode("\n", $errors));
			return false;
		}
		$this->addToolbar();

		parent::display($tpl);
	}

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
		if ($user->authorise('core.create', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')) {
			JToolBarHelper::addNew('<#= Value("Task.nameObject") #>.add','JTOOLBAR_NEW');
		}
		<# End If #>
		
		<# if StrConv(Value("Task.canEdit"), VbStrConv.Lowercase) = "true" Then #>
		if ($user->authorise('core.edit', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')) {
			JToolBarHelper::editList('<#= Value("Task.nameObject") #>.edit','JTOOLBAR_EDIT');
		}
		<# End If #>
				
		if($user->authorise('core.delete', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')) {
			JToolBarHelper::deleteList('', '<#= Value("Task.nameObjectList") #>.delete');	
		}
		
		if( (isset(current($this->items)->published)) && ($user->authorise('core.edit.state', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')) ){
			JToolBarHelper::divider();
			JToolBarHelper::publishList('<#= Value("Task.nameObjectList") #>.publish');
			JToolBarHelper::unpublishList('<#= Value("Task.nameObjectList") #>.unpublish');
		}
		
		if($user->authorise('core.admin', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')){
			JToolBarHelper::divider();
			JToolBarHelper::preferences('com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>');
		}
	}
}