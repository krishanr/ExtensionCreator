<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<?php
/**
 * <#= Value("Extension.name") #> View for com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 *
 */

// No direct access
defined('_JEXEC') or die( 'Restricted access' );

/**
 * HTML View class for the <#= Value("Extension.name") #> Component
 *
 * @package	Joomla.Components
 * @subpackage	<#= Value("Extension.name") #>
 */
class <#= Value("Extension.name") #>View<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JViewLegacy
{
	public function display($tpl = null) 
	{
		// Initialise variables.
		$app		= JFactory::getApplication();
		$user		= JFactory::getUser();

		// Get model data.
		$this->state		= $this->get('State');
		$this->showSuccess  = $this->state->get($this->getName().'.id', 0);
		$this->formName     = $this->get('FormName');
		$this->form			= $this->get('Form');
		//Get Option manually
		$this->option       = $this->getOption();

		// Check for errors.
		if (count($errors = $this->get('Errors'))) 
		{
			JError::raiseError(500, implode('<br />', $errors));
			return false;
		}
		
		$this->params	= &$this->state->params;
		$this->user		= $user;

		if($this->showSuccess) {
			//NOTE: Don't show private data here
			$this->setLayout('success');	
		} 
		$this->_prepareDocument();
		parent::display($tpl);
	}

	/**
	 * Method to set up the document properties
	 *
	 * @return void
	 */
	protected function _prepareDocument() 
	{		
		$app		= JFactory::getApplication();
		$menus		= $app->getMenu();
		$title 		= null;
		$default_title = JText::_('<#= JExt.GenText(Value("Task.nameObject") & "_PAGETITLE_DEFAULT", StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " Form") #>');
		$success_title = JText::_('<#= JExt.GenText(Value("Task.nameObject") & "_PAGETITLE_SUCCESS", StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " Form Successfully Submitted") #>');
		
		// Because the application sets a default page title,
		// we need to get it from the menu item itself
		$menu = $menus->getActive();
		if ($menu)
		{
			$this->params->def('page_heading', $this->params->get('page_title', $menu->title));
		} else {
			$this->params->def('page_heading', $default_title);
		}
		
		$title = $this->params->def('page_title', $default_title);	

		if ($app->getCfg('sitename_pagetitles', 0) == 1) {
			$title = JText::sprintf('JPAGETITLE', $app->getCfg('sitename'), $title);
		}
		elseif ($app->getCfg('sitename_pagetitles', 0) == 2) {
			$title = JText::sprintf('JPAGETITLE', $title, $app->getCfg('sitename'));
		}
		if($this->showSuccess) {	
			$title = $success_title;
		}
		$this->document->setTitle($title);

		if ($this->params->get('menu-meta_description'))
		{
			$this->document->setDescription($this->params->get('menu-meta_description'));
		}

		if ($this->params->get('menu-meta_keywords'))
		{
			$this->document->setMetadata('keywords', $this->params->get('menu-meta_keywords'));
		}

		if ($this->params->get('robots'))
		{
			$this->document->setMetadata('robots', $this->params->get('robots'));
		}
	}
	
	public function getOption($default = null) {
		// If $model is null we use the default model
		if (is_null($default)) {
			$model = $this->_defaultModel;
		} else {
			$model = strtolower($default);
		}

		// First check to make sure the model requested exists
		if (isset($this->_models[$model]))
		{
			return $this->_models[$model]->get('option');
		} else {
			return '';
		}
	}
}
