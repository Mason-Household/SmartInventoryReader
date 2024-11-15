import React from 'react';
import { IconButton, Tooltip } from '@mui/material';
import HelpOutlineIcon from '@mui/icons-material/HelpOutline';

const HelpOutline: React.FC = () => {
	return (
		<Tooltip title="Help">
			<IconButton>
				<HelpOutlineIcon />
			</IconButton>
		</Tooltip>
	);
};

export default HelpOutline;