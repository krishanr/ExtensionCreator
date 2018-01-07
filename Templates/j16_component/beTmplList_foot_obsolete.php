<#@ template inherits="ETemplate" language="VB" #>
<?php
/**
 * default foot template file for <#= Value("Task.nameObjectList") #> view of <#= Value("Extension.name") #> component
 * 
 * @package    <#= Value("Extension.name") #>
 * @subpackage com_<#= StrConv(Value("Extension.name"), VbStrConv.Lowercase) #>
 * @license  <#= Value("Extension.license") #>
 */
// No direct access to this file
defined('_JEXEC') or die('Restricted Access');
?>
<tr>
	<td colspan="<?php echo $this->NumCols; ?>"><?php echo $this->pagination->getListFooter(); ?></td>
</tr>
