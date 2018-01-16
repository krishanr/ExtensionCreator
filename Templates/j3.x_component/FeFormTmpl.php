<#@ template inherits="ETemplate" language="VB" #>
<?php
// No direct access
defined('_JEXEC') or die('Restricted access');

//TODO: remove the ones that aren't used.
JHtml::_('behavior.tabstate');
JHtml::_('behavior.keepalive');
JHtml::_('behavior.formvalidator');
JHtml::_('formbehavior.chosen', 'select');

JFactory::getDocument()->addScriptDeclaration("
	Joomla.submitbutton = function(task)
	{
		if (task == '<#= Value("Task.nameObject") #>.cancel' || document.formvalidator.isValid(document.getElementById('adminForm')))
		{
			Joomla.submitform(task, document.getElementById('adminForm')));
		}
	}
");
?>
<div class="edit">
	<form action="<?php echo JRoute::_('index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>'); ?>" method="post" name="<?php echo $this->formName; ?>Form" id="adminForm">
		<div class="tab-content">
			<div class="tab-pane active">
		        <?php
				// Iterate through the fieldsets and display them.
				 foreach ($this->form->getFieldsets() as $fieldsets => $fieldset) : 
				    ?>
				<div>
					<?php echo $this->form->renderFieldset($fieldset->name); ?>
				</div>
				    <?php
				endforeach;
				?>
			</div>
		</div>
		<input class="button" type="submit" value="Submit"/>			
		<input type="hidden" name="task" value="<?php echo $this->formName; ?>.submit" />
		<?php //id fied sent as 0, so system thinks this is a new item ?>
		<input type="hidden" name="id" value="0" />
		<?php echo JHtml::_('form.token'); ?>
	</form>
</div>