import React, { useState } from 'react';
import {
	Box,
	Card,
	CardContent,
	Typography,
	List,
	ListItem,
	ListItemText,
	Chip,
	IconButton,
	Collapse,
	TextField,
	InputAdornment,
	Button,
	Dialog,
	DialogTitle,
	DialogContent,
	DialogActions,
	useTheme,
	alpha,
} from '@mui/material';
import {
	ChevronDown,
	ChevronUp,
	Search,
	Filter,
	Share,
	Download,
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import type { MenuItem } from '../../interfaces/MenuItem';

interface MenuResultsProps {
	menuItems: MenuItem[];
	venueName?: string;
	menuType?: 'food' | 'drinks' | 'combined';
}

const MenuResults: React.FC<MenuResultsProps> = ({
	menuItems,
	venueName,
	menuType = 'combined',
}) => {
	const theme = useTheme();
	const [expanded, setExpanded] = useState<string[]>([]);
	const [searchTerm, setSearchTerm] = useState('');
	const [filterDialogOpen, setFilterDialogOpen] = useState(false);
	const [priceRange, setPriceRange] = useState<[number, number]>([0, 1000]);
	const [selectedCategories, setSelectedCategories] = useState<string[]>([]);

	const categories = Array.from(
		new Set(menuItems.map(item => item.category).filter(Boolean))
	);

	const toggleCategory = (category: string) => {
		if (expanded.includes(category)) {
			setExpanded(expanded.filter(c => c !== category));
		} else {
			setExpanded([...expanded, category]);
		}
	};

	const filteredItems = menuItems.filter(item => {
		const matchesSearch = item.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
			item.description?.toLowerCase().includes(searchTerm.toLowerCase());
		const matchesPrice = item.price >= priceRange[0] && item.price <= priceRange[1];
		const matchesCategory = selectedCategories.length === 0 ||
			(item.category && selectedCategories.includes(item.category));
		return matchesSearch && matchesPrice && matchesCategory;
	});

	const groupedItems = filteredItems.reduce((acc, item) => {
		const category = item.category || 'Uncategorized';
		if (!acc[category]) {
			acc[category] = [];
		}
		acc[category].push(item);
		return acc;
	}, {} as Record<string, MenuItem[]>);

	return (
		<Card
			component={motion.div}
			initial={{ opacity: 0, y: 20 }}
			animate={{ opacity: 1, y: 0 }}
			sx={{ mb: 3 }}
		>
			<CardContent>
				{venueName && (
					<Typography variant="h5" gutterBottom>
						{venueName}
					</Typography>
				)}

				<Box sx={{ mb: 3, display: 'flex', gap: 2, flexWrap: 'wrap' }}>
					<TextField
						size="small"
						placeholder="Search menu items..."
						value={searchTerm}
						onChange={(e) => setSearchTerm(e.target.value)}
						InputProps={{
							startAdornment: (
								<InputAdornment position="start">
									<Search size={20} />
								</InputAdornment>
							),
						}}
						sx={{ flex: 1 }}
					/>
					<Button
						variant="outlined"
						startIcon={<Filter size={20} />}
						onClick={() => setFilterDialogOpen(true)}
					>
						Filters
					</Button>
					<IconButton>
						<Share size={20} />
					</IconButton>
					<IconButton>
						<Download size={20} />
					</IconButton>
				</Box>

				<Box sx={{ display: 'flex', gap: 1, mb: 3, flexWrap: 'wrap' }}>
					<Chip
						label={`${menuType.charAt(0).toUpperCase() + menuType.slice(1)} Menu`}
						color="primary"
						variant="outlined"
					/>
					{selectedCategories.map(category => (
						<Chip
							key={category}
							label={category}
							onDelete={() => setSelectedCategories(
								selectedCategories.filter(c => c !== category)
							)}
							color="secondary"
						/>
					))}
				</Box>

				<List>
					<AnimatePresence>
						{Object.entries(groupedItems).map(([category, items]) => (
							<motion.div
								key={category}
								initial={{ opacity: 0 }}
								animate={{ opacity: 1 }}
								exit={{ opacity: 0 }}
							>
								<ListItem
									onClick={() => toggleCategory(category)}
									sx={{
										bgcolor: alpha(theme.palette.primary.main, 0.05),
										borderRadius: 1,
										mb: 1,
									}}
								>
									<ListItemText
										primary={category}
										secondary={`${items.length} items`}
									/>
									{expanded.includes(category) ? (
										<ChevronUp size={20} />
									) : (
										<ChevronDown size={20} />
									)}
								</ListItem>
								<Collapse in={expanded.includes(category)}>
									<List disablePadding>
										{items.map((item, index) => (
											<ListItem
												key={`${category}-${index}`}
												sx={{
													pl: 4,
													borderLeft: `2px solid ${alpha(
														theme.palette.primary.main,
														0.2
													)}`,
													ml: 2,
												}}
											>
												<ListItemText
													primary={
														<Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
															<Typography variant="subtitle1">
																{item.name}
															</Typography>
															<Typography
																variant="subtitle1"
																color="primary"
																fontWeight="bold"
															>
																${item.price.toFixed(2)}
															</Typography>
														</Box>
													}
													secondary={item.description}
												/>
											</ListItem>
										))}
									</List>
								</Collapse>
							</motion.div>
						))}
					</AnimatePresence>
				</List>

				<Dialog
					open={filterDialogOpen}
					onClose={() => setFilterDialogOpen(false)}
					maxWidth="sm"
					fullWidth
				>
					<DialogTitle>Filter Menu Items</DialogTitle>
					<DialogContent>
						<Box sx={{ pt: 2 }}>
							<Typography variant="subtitle2" gutterBottom>
								Categories
							</Typography>
							<Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 3 }}>
								{categories.map(category => (
									<Chip
										key={category}
										label={category}
										onClick={() => setSelectedCategories(
											selectedCategories.includes(category as string)
												? selectedCategories.filter(c => c !== category)
												: [...selectedCategories, category as string]
										)}
										color={selectedCategories.includes(category as string) ? "primary" : "default"}
									/>
								))}
							</Box>

							<Typography variant="subtitle2" gutterBottom>
								Price Range
							</Typography>
							<Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
								<TextField
									type="number"
									size="small"
									label="Min"
									value={priceRange[0]}
									onChange={(e) => setPriceRange([
										Number(e.target.value),
										priceRange[1]
									])}
								/>
								<TextField
									type="number"
									size="small"
									label="Max"
									value={priceRange[1]}
									onChange={(e) => setPriceRange([
										priceRange[0],
										Number(e.target.value)
									])}
								/>
							</Box>
						</Box>
					</DialogContent>
					<DialogActions>
						<Button onClick={() => setFilterDialogOpen(false)}>
							Cancel
						</Button>
						<Button
							variant="contained"
							onClick={() => setFilterDialogOpen(false)}
						>
							Apply Filters
						</Button>
					</DialogActions>
				</Dialog>
			</CardContent>
		</Card>
	);
};

export default MenuResults;