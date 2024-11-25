import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Tooltip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Switch,
  FormControlLabel,
} from '@mui/material';
import { Edit, DollarSign } from 'lucide-react';
import { Consigner, UpsertConsignerRequest, RecordPayoutRequest } from '../../interfaces/Consigner';

const Consigners: React.FC = () => {
  const [consigners, setConsigners] = useState<Consigner[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [payoutDialogOpen, setPayoutDialogOpen] = useState(false);
  const [selectedConsigner, setSelectedConsigner] = useState<Consigner | null>(null);
  const [formData, setFormData] = useState<UpsertConsignerRequest>({
    name: '',
    email: '',
    phone: '',
    paymentDetails: '',
    commissionRate: 0.7,
    notes: '',
    isActive: true,
  });
  const [payoutData, setPayoutData] = useState<RecordPayoutRequest>({
    amount: 0,
    paymentMethod: '',
    transactionReference: '',
    notes: '',
  });

  useEffect(() => {
    fetchConsigners();
  }, []);

  const fetchConsigners = async () => {
    try {
      const response = await fetch('/api/v1/consigners/getConsigners');
      if (!response.ok) throw new Error('Failed to fetch consigners');
      const data = await response.json();
      setConsigners(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleEditClick = (consigner: Consigner) => {
    setSelectedConsigner(consigner);
    setFormData({
      id: consigner.id,
      name: consigner.name,
      email: consigner.email,
      phone: consigner.phone,
      paymentDetails: consigner.paymentDetails,
      commissionRate: consigner.commissionRate,
      notes: consigner.notes,
      isActive: consigner.isActive,
    });
    setEditDialogOpen(true);
  };

  const handlePayoutClick = (consigner: Consigner) => {
    setSelectedConsigner(consigner);
    setPayoutData({
      amount: consigner.unpaidBalance,
      paymentMethod: '',
      transactionReference: '',
      notes: '',
    });
    setPayoutDialogOpen(true);
  };

  const handleSaveConsigner = async () => {
    try {
      const response = await fetch('/api/v1/consigners/getConsigners', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData),
      });
      if (!response.ok) throw new Error('Failed to save consigner');
      await fetchConsigners();
      setEditDialogOpen(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const handleRecordPayout = async () => {
    if (!selectedConsigner) return;
    try {
      const response = await fetch(`/api/v1/consigners/getConsigners/${selectedConsigner.id}/payouts`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payoutData),
      });
      if (!response.ok) throw new Error('Failed to record payout');
      await fetchConsigners();
      setPayoutDialogOpen(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  if (loading) return <Typography>Loading...</Typography>;
  if (error) return <Typography color="error">{error}</Typography>;

  return (
    <Box sx={{ width: '100%', maxWidth: 1200, mx: 'auto' }}>
      <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4" component="h1">Consigners</Typography>
        <Button
          variant="contained"
          onClick={() => {
            setSelectedConsigner(null);
            setFormData({
              name: '',
              email: '',
              phone: '',
              paymentDetails: '',
              commissionRate: 0.7,
              notes: '',
              isActive: true,
            });
            setEditDialogOpen(true);
          }}
        >
          Add Consigner
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Contact</TableCell>
              <TableCell align="right">Unpaid Balance</TableCell>
              <TableCell align="right">Total Paid Out</TableCell>
              <TableCell align="right">Commission Rate</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {consigners.map((consigner) => (
              <TableRow key={consigner.id}>
                <TableCell>{consigner.name}</TableCell>
                <TableCell>
                  {consigner.email && <div>{consigner.email}</div>}
                  {consigner.phone && <div>{consigner.phone}</div>}
                </TableCell>
                <TableCell align="right">${consigner.unpaidBalance.toFixed(2)}</TableCell>
                <TableCell align="right">${consigner.totalPaidOut.toFixed(2)}</TableCell>
                <TableCell align="right">{(consigner.commissionRate * 100).toFixed(0)}%</TableCell>
                <TableCell>{consigner.isActive ? 'Active' : 'Inactive'}</TableCell>
                <TableCell align="right">
                  <Tooltip title="Edit">
                    <IconButton onClick={() => handleEditClick(consigner)}>
                      <Edit size={20} />
                    </IconButton>
                  </Tooltip>
                  {consigner.unpaidBalance > 0 && (
                    <Tooltip title="Record Payout">
                      <IconButton onClick={() => handlePayoutClick(consigner)}>
                        <DollarSign size={20} />
                      </IconButton>
                    </Tooltip>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Edit Dialog */}
      <Dialog open={editDialogOpen} onClose={() => setEditDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{selectedConsigner ? 'Edit Consigner' : 'Add Consigner'}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
            <TextField
              label="Name"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              fullWidth
            />
            <TextField
              label="Phone"
              value={formData.phone}
              onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
              fullWidth
            />
            <TextField
              label="Payment Details"
              value={formData.paymentDetails}
              onChange={(e) => setFormData({ ...formData, paymentDetails: e.target.value })}
              fullWidth
              multiline
              rows={2}
            />
            <TextField
              label="Commission Rate (%)"
              type="number"
              value={formData.commissionRate * 100}
              onChange={(e) => setFormData({ ...formData, commissionRate: Number(e.target.value) / 100 })}
              fullWidth
              required
            />
            <TextField
              label="Notes"
              value={formData.notes}
              onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
              fullWidth
              multiline
              rows={3}
            />
            <FormControlLabel
              control={
                <Switch
                  checked={formData.isActive}
                  onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                />
              }
              label="Active"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleSaveConsigner} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Payout Dialog */}
      <Dialog open={payoutDialogOpen} onClose={() => setPayoutDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Record Payout</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
            <Typography>
              Recording payout for: {selectedConsigner?.name}
            </Typography>
            <TextField
              label="Amount"
              type="number"
              value={payoutData.amount}
              onChange={(e) => setPayoutData({ ...payoutData, amount: Number(e.target.value) })}
              fullWidth
              required
            />
            <TextField
              label="Payment Method"
              value={payoutData.paymentMethod}
              onChange={(e) => setPayoutData({ ...payoutData, paymentMethod: e.target.value })}
              fullWidth
            />
            <TextField
              label="Transaction Reference"
              value={payoutData.transactionReference}
              onChange={(e) => setPayoutData({ ...payoutData, transactionReference: e.target.value })}
              fullWidth
            />
            <TextField
              label="Notes"
              value={payoutData.notes}
              onChange={(e) => setPayoutData({ ...payoutData, notes: e.target.value })}
              fullWidth
              multiline
              rows={3}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPayoutDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleRecordPayout} variant="contained">Record Payout</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Consigners;
