<#@ template inherits="ETemplate" language="VB" #>
<?php
// No direct access.
defined('_JEXEC') or die;

// Load the tooltip behavior.
JHtml::_('behavior.tooltip');
JHtml::_('behavior.formvalidation');
JHtml::_('behavior.keepalive');
?>

<script type="text/javascript">
	Joomla.submitbutton = function(task) {
		if (task == '<#= Value("Task.nameObject") #>.cancel' || document.formvalidator.isValid(document.id('item-form'))) {
			Joomla.submitform(task, document.getElementById('item-form'));
		} else {
			alert('<?php echo $this->escape(JText::_('JGLOBAL_VALIDATION_FORM_FAILED'));?>');
		}
	}
</script>

<form action="<?php echo JRoute::_('index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>&layout=edit&id='.(int) $this->item->id); ?>" method="post" name="adminForm" id="item-form" class="form-validate">
	<?php foreach ($this->form->getFieldsets() as $fieldsets => $fieldset) : ?>
    <fieldset class="adminform">
        <legend>
            <?php echo $fieldset->label; ?>
        </legend>
        <ul class="adminformlist">
	        <?php
			// Iterate through the fields and display them.
			foreach($this->form->getFieldset($fieldset->name) as $field) :
			    // If the field is hidden, only use the input.
			    if ($field->hidden):
			        echo $field->input;
			    else:
			    ?>
				    <li>
					    <?php echo $field->label; ?>
					    <?php echo $field->input ?>
				    </li>
			    <?php
			    endif;
			endforeach;
			?>
		</ul>
    </fieldset>
	<?php endforeach; ?>
	<div>
		<input type="hidden" name="task" value="" />
		<input type="hidden" name="return" value="<?php echo JRequest::getCmd('return');?>" />
		<?php echo JHtml::_('form.token'); ?>
	</div>
</form>