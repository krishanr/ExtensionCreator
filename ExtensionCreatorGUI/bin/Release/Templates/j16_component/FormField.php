<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * <#= Value("Task.name") #> Field for <#= Value("Extension.name") #> Component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @copyright	<#= Value("Extension.copyright") #>
 * @license  <#= Value("Extension.license") #>
 */
// See http://docs.joomla.org/Overriding_JFormFields if you need more help.
// Check to ensure this file is included in Joomla!
defined('_JEXEC') or die();

jimport('joomla.html.html');
jimport('joomla.form.formfield');
<# If Value("Task.parent") <> "" Then #>
jimport('joomla.form.helper');
JFormHelper::loadFieldClass('<#= StrConv(Value("Task.parent"), VbStrConv.Lowercase) #>');
<# End If #>

/**
 * <#= Value("Task.name") #> Field
 *
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 */
class JFormField<#= Value("Task.name") #> extends JFormField<#= Value("Task.parent") #> {
 
	//The field class must know its own type through the variable $type.
	protected $type = '<#= Value("Task.name") #>';

	protected function setProperty($name, $default = NULL ) {
		$this->$name = ($this->element[$name]) ? $this->element[$name] : $default;
	}
	
	// Inside your code, you will have to process the attributes set by the field's user
	// in the XML form definition. Some of those attributes are accessible via protected 
	// member variables of JFormField. For example, the name attribute is available in your
	// code as $this->name. Similarly, label, description, default, multiple and class are 
	// also available as properties of $this. Other parameters you might have defined can 
	// be accessed through the $this->element array: the attribute size will be in 
	// $this->element['size'].
 
	/* Uncomment this and use it if you wish.
	protected function getLabel() {
		// This function will be called to create the label that belongs to your field 
		// and must return a HTML string containing it. Since JFormField defines a 
		// ready-to-use getLabel() implementation, custom form field types usually do not 
		// define their own getLabel(). If you leave it out, the inherited method of creating
		// labels will be used. It is recommended to leave out the getLabel() method for
		// consistency and speed unless you actually want to modify the label's HTML.
	}
 
	protected function getInput() {
        ob_start();  
		// This function will be called to create the field itself and must return a HTML 
		// string containing it. This is also where most of the processing usually happens.
		// In our phone book City field example, this function will have to retrieve a list
		// of available cities and return a HTML <select> with the cities inserted as <option>s.    
        ?>  
        <!-- Html goes here -->
        <?php
        $content    = ob_get_contents();
        ob_end_clean();
        
        return $content;
	}
	
<# If StrConv(Value("Task.parent"), VbStrConv.Lowercase).Contains("list") Then #>
	protected function getOptions()
	{
		// Initialize variables.
		$options = array();

		// Prepend some default options
		$options[] = JHtml::_('select.option', '-1', JText::_('KEY_FOR_OPTION'), '_', $this->name)));

		// Merge any additional options in the XML definition.
		$options = array_merge(parent::getOptions(), $options);

		return $options;
	}
<# End If #>
	*/
}