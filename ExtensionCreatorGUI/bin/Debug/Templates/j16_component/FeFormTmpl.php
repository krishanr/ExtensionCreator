<#@ template inherits="ETemplate" language="VB" #>
<?php
// No direct access
defined('_JEXEC') or die('Restricted access');

//Register the Jtip html helper.
//TODO: try using jimport instead
JLoader::register('JHtmlMyBehavior', JPATH_LIBRARIES . '/mylibrary/html/html/mybehavior.php');

//Add CSS and Javascript here.
$basePath =  JURI::root(true).'/media/<#= Value("Extension.fullName") #>';
$doc = JFactory::getDocument();
JHtml::_('MyBehavior.jquery');
//TODO: should check if the js file was added to the list of folders ideally..
$doc->addScript("$basePath/js/$this->formName.js");
?>
<?php //TODO: Change layout class from contact-form to more general, and css ?>
<div class="contact-form my-form">
	<p>All fields marked with a<span class="star">&nbsp;*</span> are required.</p>
	<form action="<?php echo JRoute::_('index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>'); ?>" method="post" name="<?php echo $this->formName; ?>Form" id="<?php echo $this->formName; ?>Form">
		<?php foreach ($this->form->getFieldsets() as $fieldsets => $fieldset) : ?>
	    <fieldset class="my-form-fieldset">
	        <legend>
	            <?php echo $fieldset->label; ?>
	        </legend>
	        <dl>
		        <?php
				// Iterate through the fields and display them.
				foreach($this->form->getFieldset($fieldset->name) as $field) :
				    // If the field is hidden, only use the input.
				    if ($field->hidden):
				        echo $field->input;
				    else:
				    ?>
					    <dt><?php echo $field->label; ?></dt>
					    <dd><?php echo $field->input ?></dd>
				    <?php
				    endif;
				endforeach;
				?>
			</dl>
	    </fieldset>
		<?php endforeach; ?>
		<input class="button" type="submit" value="Submit"/>			
		<input type="hidden" name="task" value="<?php echo $this->formName; ?>.submit" />
		<?php //id fied sent as 0, so system thinks this is a new item ?>
		<input type="hidden" name="id" value="0" />
		<?php echo JHtml::_('form.token'); ?>
	</form>
</div>