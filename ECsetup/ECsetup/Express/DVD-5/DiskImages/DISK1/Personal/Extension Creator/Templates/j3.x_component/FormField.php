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

<# If Value("Task.parent") <> "" Then #>
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
	//
	// * Cached array of the <#= Value("Task.name") #> items.
	// *
	// * @var    array
	// * @since  3.2
	 //
	protected static $options = array();

	// SAMPLE code to load options statically
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
	
	// SAMPLE code to load options from a databse
	//*
	// * Method to get the options to populate list
	// *
	// * @return  array  The field option objects.
	// *
	// * @since   3.2
	// 
	protected function getOptions()
	{
		// Accepted modifiers
		$hash = md5($this->element);

		if (!isset(static::$options[$hash]))
		{
			static::$options[$hash] = parent::getOptions();

			$options = array();

			$db = JFactory::getDbo();

			// Construct the query
			$query = $db->getQuery(true)
				->select('a.<#= Value("Task.value") #> AS value, a.<#= Value("Task.text") #> AS text')
				->from('#__<#= Value("Task.table") #> AS a')
<#				   If Value("Task.published") = "true" Then #>
				->where('a.published = 1')
<#				   End If  #>
				->group('a.<#= Value("Task.value") #>')
				->order('a.<#= Value("Task.text") #>');
		
			// Setup the query
			$db->setQuery($query);

			// Return the result
			if ($options = $db->loadObjectList())
			{
				static::$options[$hash] = array_merge(static::$options[$hash], $options);
			}
		}

		return static::$options[$hash];
	}
	//The following code would be used in the SQL query. Make sure to also add a line to populateState in the model.
	//$<#= Value("Task.text") #> = $this->state->get('filter.<#= Value("Task.text") #>', '');
	//	if($<#= Value("Task.text") #> != '') {
	//		$<#= Value("Task.text") #> = $db->quote($db->escape(trim($<#= Value("Task.text") #>), true));
	//		$query->where('a.<#= Value("Task.value") #> = ' . $<#= Value("Task.text") #>);
	//	}		

<# End If #>
	*/
}