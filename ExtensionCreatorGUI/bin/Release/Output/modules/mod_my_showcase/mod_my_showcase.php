<?php
/**
 * @version		1.0.0
 * @package		Joomla.site
 * @subpackage	mod_my_showcase
 * @copyright	Copyright (C) 2005 - 2016 Open Source Matters. All rights reserved.
 * @license		GNU General Public License version 2 or later
 *  */

// no direct access 
defined('_JEXEC') or die;

// Include the syndicate functions only once
require_once dirname(__FILE__).'/helper.php';

$moduleclass_sfx = htmlspecialchars($params->get('moduleclass_sfx'));

require JModuleHelper::getLayoutPath('mod_my_showcase', $params->get('layout', 'default'));
