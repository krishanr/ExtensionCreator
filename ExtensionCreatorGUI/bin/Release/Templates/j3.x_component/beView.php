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
defined('_JEXEC') or die( 'Restricted access' );

/**
 * <#= Value("Task.nameObject") #> view
 *
 * @since  3.1
 */
class <#= Value("Extension.name") #>View<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JViewLegacy
{
	protected $form;
	protected $item;
	protected $state;

	/**
	 * Display the view
	 */
	public function display($tpl = null)
	{
		// Initialiase variables.
		$this->form		= $this->get('Form');
		$this->item		= $this->get('Item');
		$this->state	= $this->get('State');

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
		JFactory::getApplication()->input->set('hidemainmenu', true);
		$user = JFactory::getUser();
		$isNew = (empty($this->item-><#= Value("Task.primaryKey") #>));
<#      Dim myTempNameObject as string = StrConv(Value("Task.nameObject"), VbStrConv.ProperCase)
		Dim NewManagerKey as string = JExt.GenText("manager " & myTempNameObject & " new", myTempNameObject & " Manager: New " & myTempNameObject)
		Dim EditManagerKey as string = JExt.GenText("manager " & myTempNameObject & " edit", myTempNameObject & " Manager: Edit " & myTempNameObject) #>
		JToolBarHelper::title($isNew ? JText::_('<#= NewManagerKey #>') : JText::_('<#= EditManagerKey #>'), 'generic.png');

		// For new records, check the create permission.
		//TODO: should have a checked out feature
		if ($isNew && $user->authorise('core.create', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>'))  {
			JToolBarHelper::apply('<#= Value("Task.nameObject") #>.apply', 'JTOOLBAR_APPLY');
			JToolBarHelper::save('<#= Value("Task.nameObject") #>.save', 'JTOOLBAR_SAVE');
			JToolBarHelper::custom('<#= Value("Task.nameObject") #>.save2new', 'save-new.png', 'save-new_f2.png', 'JTOOLBAR_SAVE_AND_NEW', false);
			JToolBarHelper::cancel('<#= Value("Task.nameObject") #>.cancel', 'JTOOLBAR_CANCEL');
		} else {
			// Since it's an existing record, check the edit permission
			if($user->authorise('core.edit', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>')) {
				JToolBarHelper::apply('<#= Value("Task.nameObject") #>.apply', 'JTOOLBAR_APPLY');
				JToolBarHelper::save('<#= Value("Task.nameObject") #>.save', 'JTOOLBAR_SAVE');	
				// We can save this record, but check the create permission to see if we can return to make a new one.
				if ($user->authorise('core.create', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>'))  {
					JToolBarHelper::custom('<#= Value("Task.nameObject") #>.save2new', 'save-new.png', 'save-new_f2.png', 'JTOOLBAR_SAVE_AND_NEW', false);
				}
			}
	
			if ($user->authorise('core.create', 'com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>'))  {
				JToolBarHelper::custom('<#= Value("Task.nameObject") #>.save2copy', 'save-copy.png', 'save-copy_f2.png', 'JTOOLBAR_SAVE_AS_COPY', false);
			}
			
			// for existing items the button is renamed 'close'
			JToolBarHelper::cancel( '<#= Value("Task.nameObject") #>.cancel', 'JTOOLBAR_CLOSE' );
		}
	}
}