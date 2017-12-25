<#@ template inherits="ETemplate" language="VB" #>
<?php // no direct access
defined('_JEXEC') or die('Restricted access'); ?>
<table>
	<thead>
		<tr>
			<th>
				View
			</th>
<!-- Joomla! Component Builder - begin code  -->
!fieldheaderlist!
<!-- Joomla! Component Builder - begin code  -->
		</tr>
	</thead>
	<tfoot>
		<tr>
			<td colspan="0">
				<div class="jcb_pagination"><?php echo $this->pagination->getPagesLinks(); ?> - <?php echo $this->pagination->getPagesCounter(); ?></div>
			</td>
		</tr>
	</tfoot>
	<tbody>
<?php foreach($this->data as $dataItem): ?> 
<?php
	$link = JRoute::_( "index.php?option=com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>&view=<#= Value("Task.nameObject") #>&id={$dataItem-><#= Value("Task.primaryKey") #>}" );
?>
		<tr>
			<td>
				<!-- You can use $link var for link edit controller -->
				<a href="<?php echo $link; ?>">View</a>
			</td>
<!-- Joomla! Component Builder - begin code  -->
!fieldlist!
<!-- Joomla! Component Builder - begin code  -->
		</tr>
<?php endforeach; ?>
	<tbody>
</table>

