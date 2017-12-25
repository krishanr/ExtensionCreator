<#@ template inherits="ETemplate" language="VB" #>
<# Dim JExt as JExtension = CType(Ext, JExtension) #>
<?php
// no direct access
defined('_JEXEC') or die;

jimport('joomla.application.component.controllerform');

class <#= Value("Extension.name") #>Controller<#= StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) #> extends JControllerForm
{		
	/**
	 * @var		string	The prefix to use with controller messages.
	 * @since	1.6
	 */
	protected $text_prefix = 'COM_<#= StrConv(Value("Extension.name") & "_" & Value("Task.nameObject"), VbStrConv.UpperCase) #>';
	<# 'Set the controller messages.' #>
	<# JExt.GenText(Value("Task.nameObject") & " submit save success", StrConv(Value("Task.nameObject"), VbStrConv.ProperCase) & " form successfully submitted.") #>
	
	/**
	 * Method to save form data.
	 * 
	 * @return  boolean  True if successful, false otherwise.
	 */
	public function submit()
	{
		// Check for request forgeries.
		JRequest::checkToken() or jexit(JText::_('JINVALID_TOKEN'));

		// Initialise variables.
		$app		= JFactory::getApplication();
		$lang		= JFactory::getLanguage();
		$model		= $this->getModel();
		$table		= $model->getTable();
		$data		= JRequest::getVar('jform', array(), 'post', 'array');
		$context	= "$this->option.edit.$this->context";
		$params     = $app->getParams();

		//Load the admin language file for save errors		
		$lang->load($this->option, JPATH_ADMINISTRATOR, 'en-GB', true);
		
		// Determine the name of the primary key for the data.
		$key = $table->getKeyName();

		$recordId	= JRequest::getInt($key);

		// Populate the row id from the session.
		$data[$key] = $recordId;

		// Validate the posted data.
		// Sometimes the form needs some posted data, such as for plugins and modules.
		$form = $model->getForm($data, false);

		if (!$form) {
			$app->enqueueMessage($model->getError(), 'error');

			return false;
		}

		// Test whether the data is valid.
		$validData = $model->validate($form, $data);

		// Check for validation errors.
		if ($validData === false) {
			// Get the validation messages.
			$errors	= $model->getErrors();

			// Push up to three validation messages out to the user.
			for ($i = 0, $n = count($errors); $i < $n && $i < 3; $i++)
			{
				if (JError::isError($errors[$i])) {
					$app->enqueueMessage($errors[$i]->getMessage(), 'warning');
				}
				else {
					$app->enqueueMessage($errors[$i], 'warning');
				}
			}

			// Save the data in the session.
			$app->setUserState($context.'.data', $data);

			// Redirect back to the edit screen.
			$this->setRedirect(JRoute::_('index.php?option='.$this->option.'&view='.$this->view_item.$this->getRedirectAppend(), false));

			return false;
		}

		// Attempt to save the data.
		if (!$model->save($validData)) {
			// Save the data in the session.
			$app->setUserState($context.'.data', $validData);

			// Redirect back to the edit screen.
			$this->setError(JText::sprintf('JLIB_APPLICATION_ERROR_SAVE_FAILED', $model->getError()));
			$this->setMessage($this->getError(), 'error');
			$this->setRedirect(JRoute::_('index.php?option='.$this->option.'&view='.$this->view_item.$this->getRedirectAppend(), false));

			return false;
		}

		$this->setMessage(JText::_(($lang->hasKey($this->text_prefix.($recordId==0 && $app->isSite() ? '_SUBMIT' : '').'_SAVE_SUCCESS') ? $this->text_prefix : 'JLIB_APPLICATION') . ($recordId==0 && $app->isSite() ? '_SUBMIT' : '') . '_SAVE_SUCCESS'));

		// Redirect the user
			// Clear the data from the session.
			$app->setUserState($context.'.data', null);
			
			$redirectUrl = $params->get(<#= JExt.AddParam("redirect_url", "") #>);
			
			if(empty($redirectUrl)) {
				//Add the id to the session which will be used by the application
				$recordId   = $model->getState($model->getName().'.id');
				$app->setUserState($context.'.id', $recordId);	
				
				// Redirect to the same view.
				$this->setRedirect(JRoute::_('index.php?option='.$this->option.'&view='.$this->view_item.$this->getRedirectAppend(), false));				
			} else {
				// Redirect to the location specified by the parameter.
				$this->setRedirect($redirectUrl, false);				
			}

		// Invoke the postSave method to allow for the child class to access the model.
		$this->postSaveHook($model, $validData);

		return true;
	}
	
	/**
	 * Gets the URL arguments to append to a redirect.
	 *
	 * @return  string  The arguments to append to the redirect URL.
	 * @since   11.1
	 */
	protected function getRedirectAppend()
	{
		$tmpl		= JRequest::getCmd('tmpl');
		$layout		= JRequest::getCmd('layout', 'fill');
		$append		= '';

		// Setup redirect info.
		if ($tmpl) {
			$append .= '&tmpl='.$tmpl;
		}
		
		if ($layout) {
			$append .= '&layout='.$layout;
		}

		return $append;
	}
	
	/**
	 * Function that allows child controller access to model data after the data has been saved.
	 *
	 * @param   JModel	$model      The data model object.
	 * @param   array   $validData  The validated data.
	 *
	 * @return  void
	 * @since   11.1
	 */
	protected function postSaveHook( &$model, $validData = array())
	{
		//TODO: Notify user if sendemail fails
		//$model->sendEmail($validData);
	}
}
